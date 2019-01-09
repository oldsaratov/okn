using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;

namespace AspNet.Security.OAuth.Oldsaratov.Events
{
    public class OldsaratovAuthenticationFailedContext : ResultContext<OldsaratovAuthenticationOptions>
    {
        public OldsaratovAuthenticationFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            OldsaratovAuthenticationOptions options)
            : base(context, scheme, options)
        {
        }

        public Exception Exception { get; set; }
    }
}
