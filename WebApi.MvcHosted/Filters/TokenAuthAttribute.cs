using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http.Headers;
using System.Net;
using System.Web.Http;

namespace WebApi.MvcHosted.Filters
{
    public class TokenAuthAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Find and break down the Authorization header
            var tokenDictionary = ParseTokenAuth(actionContext.Request.Headers);

            // If it passes, we're done here
            if (tokenDictionary != null && ValidateToken(tokenDictionary))
                return;

            // Otherwise throw an Unauthorized with proper WWW-Authenticate header
            var exception = new HttpResponseException(HttpStatusCode.Unauthorized);
            exception.Response.Headers.Add("WWW-Authenticate", "Token realm=\"http://web-api-samples.com\"");
            throw exception;
        }

        private IDictionary<string, string> ParseTokenAuth(HttpRequestHeaders headers)
        {
            // No auth header
            if (headers.Authorization == null) return null;

            // Not token auth
            if (string.Compare(headers.Authorization.Scheme, "token", true) != 0) return null;

            // Parse out the actual token
            return headers.Authorization.Parameter.Split(',') // Get each chunk
                .ToDictionary(
                    x => x.Substring(0, x.IndexOf("=")).Trim().ToLower(), // Keys - names up to the equals sign
                    x => x.Substring(x.IndexOf("=") + 1).Trim(new[] { '"', ' ' })); // Values - After =, remove quotes
        }

        private bool ValidateToken(IDictionary<string, string> tokenDictionary)
        {
            var token = tokenDictionary["token"];

            // TODO: Your favorite implementation of token validation. Check a membership db? Attempt decryption? 
            return token.Length > 4;
        }
    }
}