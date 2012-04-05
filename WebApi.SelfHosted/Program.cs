using System.Web.Http;
using System.Web.Http.SelfHost;
using System;

namespace WebApi.SelfHosted
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = CreateSelfHostConfiguration();
            StartServer(config);
            
            Console.WriteLine("q to quit");
            while (string.Compare(Console.ReadLine(), "q", true) != 0);
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
