using AspNet.Security.OAuth.Oldsaratov.Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AspNet.Security.OAuth.Oldsaratov
{
    internal class OldsaratovAuthenticationHandler : AuthenticationHandler<OldsaratovAuthenticationOptions>
    {
        private readonly TokenMemoryCache _memoryCache;

        public OldsaratovAuthenticationHandler(
            TokenMemoryCache memoryCache,
            IOptionsMonitor<OldsaratovAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _memoryCache = memoryCache;
        }

        protected new OldsaratovAuthenticationEvents Events => (OldsaratovAuthenticationEvents)base.Events;

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new OldsaratovAuthenticationEvents());

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!authorizationHeader.StartsWith(OldsaratovAuthenticationDefaults.AuthenticationScheme + ' ', StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            var token = authorizationHeader.Substring(OldsaratovAuthenticationDefaults.AuthenticationScheme.Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                const string noCredentialsMessage = "No credentials";
                Logger.LogInformation(noCredentialsMessage);
                return AuthenticateResult.Fail(noCredentialsMessage);
            }

            try
            {
                var validateCredentialsContext = new ValidateCredentialsContext(Context, Scheme, Options)
                {
                    AccessToken = token
                };

                var cachedTicket = _memoryCache?.Cache?.Get<AuthenticationTicket>(token);
                if (cachedTicket != null)
                {
                    return AuthenticateResult.Success(cachedTicket);
                }

                await Events.ValidateCredentials(validateCredentialsContext);

                if (validateCredentialsContext.Result != null &&
                    validateCredentialsContext.Result.Succeeded)
                {
                    var ticket = new AuthenticationTicket(validateCredentialsContext.Principal, Scheme.Name);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(Options.TokenExpirationTimeout);

                    _memoryCache?.Cache?.Set(token, ticket, cacheEntryOptions);

                    return AuthenticateResult.Success(ticket);
                }

                if (validateCredentialsContext.Result?.Failure != null)
                {
                    return AuthenticateResult.Fail(validateCredentialsContext.Result.Failure);
                }

                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                var authenticationFailedContext = new OldsaratovAuthenticationFailedContext(Context, Scheme, Options)
                {
                    Exception = ex
                };

                await Events.AuthenticationFailed(authenticationFailedContext);

                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                return AuthenticateResult.Fail(ex);
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;

            return Task.CompletedTask;
        }
    }
}