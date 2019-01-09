using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNet.Security.OAuth.Oldsaratov.Events
{
    public class ValidateCredentialsContext : ResultContext<OldsaratovAuthenticationOptions>
    {
        public ValidateCredentialsContext(
            HttpContext context,
            AuthenticationScheme scheme,
            OldsaratovAuthenticationOptions options)
            : base(context, scheme, options)
        {
        }

        public string AccessToken { get; set; }
    }
}
