using System;
using System.Web;

namespace WebApi.MvcHosted.Infrastructure
{
    // http://haacked.com/archive/2011/10/04/prevent-forms-authentication-login-page-redirect-when-you-donrsquot-want.aspx
    public class SuppressFormsAuthenticationRedirectModule : IHttpModule
    {
        private static readonly object SuppressAuthenticationKey = new Object();

        public static void SuppressAuthenticationRedirect(HttpContext context)
        {
            context.Items[SuppressAuthenticationKey] = true;
        }

        public static void SuppressAuthenticationRedirect(HttpContextBase context)
        {
            context.Items[SuppressAuthenticationKey] = true;
        }

        public void Init(HttpApplication context)
        {
            context.PostReleaseRequestState += OnPostReleaseRequestState;
            context.EndRequest += OnEndRequest;
        }

        private void OnPostReleaseRequestState(object source, EventArgs args)
        {
            var context = (HttpApplication)source;
            var response = context.Response;

            if (response.StatusCode == 401)
            {
                SuppressAuthenticationRedirect(context.Context);
            }
        }

        private void OnEndRequest(object source, EventArgs args)
        {
            var context = (HttpApplication)source;
            var response = context.Response;

            if (context.Context.Items.Contains(SuppressAuthenticationKey))
            {
                response.TrySkipIisCustomErrors = true;
                response.ClearContent();
                response.StatusCode = 401;
                response.RedirectLocation = null;
            }
        }

        public void Dispose()
        {
        }
    }
}