using System.Web;
using WebApi.MvcHosted.App_Start;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof(FormsAuthenticationConfig), "Register")]
namespace WebApi.MvcHosted.App_Start {
    using WebApi.MvcHosted.Infrastructure;

    public static class FormsAuthenticationConfig {
        public static void Register() {
            DynamicModuleUtility.RegisterModule(typeof(SuppressFormsAuthenticationRedirectModule));
        }
    }
}
