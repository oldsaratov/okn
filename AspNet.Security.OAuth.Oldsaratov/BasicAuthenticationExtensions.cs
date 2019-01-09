using AspNet.Security.OAuth.Oldsaratov;
using Microsoft.AspNetCore.Authentication;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OldsaratovAuthenticationAppBuilderExtensions
    {
        public static AuthenticationBuilder AddOldsaratov(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddOldsaratov(authenticationScheme, configureOptions: null);

        public static AuthenticationBuilder AddOldsaratov(this AuthenticationBuilder builder,
            Action<OldsaratovAuthenticationOptions> configureOptions) =>builder.AddOldsaratov(OldsaratovAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddOldsaratov(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<OldsaratovAuthenticationOptions> configureOptions)
        {
            builder.Services.AddSingleton<TokenMemoryCache>();

            return builder.AddScheme<OldsaratovAuthenticationOptions, OldsaratovAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}