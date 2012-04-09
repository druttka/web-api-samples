using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using WebApi.SelfHosted.Handlers;
using WebApi.Common;
using Autofac;
using WebApi.SelfHosted.Api.Controllers;
using System;
using WebApi.MvcHosted.Infrastructure;

namespace WebApi.MvcHosted
{
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
            // One way to do it:
            //GlobalConfiguration.Configuration.ServiceResolver.SetResolver(
            //    t =>
            //    {
            //        return (t == typeof(SpeakerController)) ? new SpeakerController(new FakeSpeakerRepository()) : null;
            //    },
            //    t =>
            //    {
            //        return (t == typeof(SpeakerController)) ? new[] { new SpeakerController(new FakeSpeakerRepository()) } : new object[] {};
            //    }
            //    );

            // Another way to do it:
            var builder = new ContainerBuilder();
            builder.RegisterType<FakeSpeakerRepository>().As<ISpeakerRepository>();
            builder.RegisterType<SpeakerController>();
            var container = builder.Build();

            //GlobalConfiguration.Configuration.ServiceResolver.SetResolver(
            //        type =>
            //        {
            //            try { return container.Resolve(type); }
            //            catch { return null; }
            //        },
            //        type =>
            //        {
            //            try { return new[] { container.Resolve(type) }; }
            //            catch { return new object[] { }; }
            //        }
            //    );

            // Another way to do it:
            GlobalConfiguration.Configuration.ServiceResolver.SetResolver(new AutofacResolver(container));
        }
    }
}