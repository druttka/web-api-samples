using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using WebApi.SelfHosted.Handlers;
using WebApi.Common;
using WebApi.SelfHosted.Api.Controllers;

namespace WebApi.MvcHosted
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            GlobalConfiguration.Configuration.MessageHandlers.Add(new InlineCountHandler());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            GlobalConfiguration.Configuration.ServiceResolver.SetResolver(
                t =>
                {
                    return (t == typeof(SpeakerController)) ? new SpeakerController(new FakeSpeakerRepository()) : null;
                },
                t =>
                {
                    return (t == typeof(SpeakerController)) ? new[] { new SpeakerController(new FakeSpeakerRepository()) } : new object[] {};
                }
                );
        }
    }
}