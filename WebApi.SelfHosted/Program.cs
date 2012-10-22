using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.SelfHost;
using System.Web.Http;

namespace WebApi.SelfHosted
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = CreateSelfHostConfiguration();
            StartServer(config);

            Console.WriteLine("ASP.NET Web API started. Press q to quit...");
            while (Console.Read() != (char)'q');
        }

        private static HttpSelfHostConfiguration CreateSelfHostConfiguration()
        {
            // Setup server configuration 
            const string baseAddress = "http://localhost:8081/";

            var config = new HttpSelfHostConfiguration(baseAddress);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            return config;
        }

        private static void StartServer(HttpSelfHostConfiguration config)
        {
            // Create and open the server 
            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
            Console.WriteLine("The server is running...");
        }
    }
}
