using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebApi.Shared;
using WebApi.MvcHosted.Infrastructure;
using WebApi.MvcHosted.Filters;

namespace WebApi.MvcHosted
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Non-template stuff below
            var config = GlobalConfiguration.Configuration;
            RegisterDependencies(config);
            RegisterFormatters(config);
        }

        private void RegisterDependencies(HttpConfiguration config)
        {
            var resolver = new AutofacResolver();
            config.DependencyResolver = resolver;

            // Note here that the System.Web.Mvc resolver is still it's own separate deal
            // For the sake of simplicity, we'll go ahead and register it here too
            DependencyResolver.SetResolver(resolver);
        }

        private void RegisterFormatters(HttpConfiguration config)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());

            var jsonpFormatter = new JsonpMediaTypeFormatter("callback") { SerializerSettings = serializerSettings };

            // Replace the default json formatter with our own (which extends it)
            var defaultJsonFormatter = config.Formatters.FirstOrDefault(
                x => x.SupportedMediaTypes.Any(s => string.Compare(s.MediaType, "application/json", true) == 0)
            );

            var index = config.Formatters.IndexOf(defaultJsonFormatter);
            if (index > -1)
                config.Formatters[index] = jsonpFormatter;
            else
                config.Formatters.Insert(0, jsonpFormatter);
        }
    }
}