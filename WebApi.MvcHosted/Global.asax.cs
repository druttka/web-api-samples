using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using WebApi.SelfHosted.Handlers;
using System;
using WebApi.MvcHosted.Infrastructure;
using Raven.Client;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebApi.MvcHosted.Filters;

namespace WebApi.MvcHosted
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static IDocumentStore _documentStore;
        public static IDocumentStore DocumentStore { get { return _documentStore; } }

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

            var config = GlobalConfiguration.Configuration;

            RegisterDependencies(config);
            RegisterFormatters(config);
            RegisterFilters(config);
        }

        private void RegisterDependencies(HttpConfiguration config)
        {
            // One way to do it:
            //config.ServiceResolver.SetResolver(
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
            //var builder = new ContainerBuilder();
            //builder.RegisterType<FakeSpeakerRepository>().As<ISpeakerRepository>();
            //builder.RegisterType<SpeakerController>();
            //var container = builder.Build();

            //config.ServiceResolver.SetResolver(
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
            var resolver = new AutofacResolver();
            config.ServiceResolver.SetResolver(resolver);

            // Note here that the System.Web.Mvc resolver is still it's own separate deal
            DependencyResolver.SetResolver(resolver);
        }

        private void RegisterFormatters(HttpConfiguration config)
        {
            // Create Json.Net formatter serializing DateTime using the ISO 8601 format
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());

            var defaultJsonFormatter = config.Formatters.First(
                x => x.SupportedMediaTypes.Any(s => string.Compare(s.MediaType, "application/json", true) == 0)
            );

            var index = config.Formatters.IndexOf(defaultJsonFormatter);
            //config.Formatters[index] = new JsonNetFormatter(serializerSettings);

            //config.Formatters[index] = new JsonpMediaTypeFormatter(serializerSettings);
        }

        private void RegisterFilters(HttpConfiguration config)
        {
            config.Filters.Add(new LogErrorFilter());
        }
    }
}