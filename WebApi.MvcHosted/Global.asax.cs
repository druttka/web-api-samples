using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using WebApi.SelfHosted.Handlers;
using System;
using WebApi.MvcHosted.Infrastructure;
using Raven.Client;
using Raven.Client.Document;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebApi.MvcHosted.Formatters;

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

            InitializeRavenDb();

            RegisterDependencies();
            RegisterFormatters(GlobalConfiguration.Configuration);

            //GlobalConfiguration.Configuration.Filters.Add(new LogErrorFilter());
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
            //var builder = new ContainerBuilder();
            //builder.RegisterType<FakeSpeakerRepository>().As<ISpeakerRepository>();
            //builder.RegisterType<SpeakerController>();
            //var container = builder.Build();

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
            GlobalConfiguration.Configuration.ServiceResolver.SetResolver(new AutofacResolver());
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
            config.Formatters[index] = new JsonNetFormatter(serializerSettings);
        }

        private void InitializeRavenDb()
        {
            _documentStore = new DocumentStore {ConnectionStringName="Raven"};
            _documentStore.Initialize();
        }
    }
}