using Microsoft.AspNetCore.Authentication;
using System;

namespace AspNet.Security.OAuth.Oldsaratov
{
    public class OldsaratovAuthenticationOptions : AuthenticationSchemeOptions
    {
        public OldsaratovAuthenticationOptions()
        {
        }

        internal string UserInformationEndpoint => "https://oldsaratov.ru/oauth2/UserInfo";

        public TimeSpan TokenExpirationTimeout { get; set; }
    }
}
