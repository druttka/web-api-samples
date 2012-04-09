using System;
using Autofac;
using WebApi.SelfHosted.Api.Controllers;
using WebApi.Common;
using Raven.Client;

namespace WebApi.MvcHosted.Infrastructure
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class AutofacResolver : System.Web.Http.Services.IDependencyResolver
    {
        private readonly IContainer _container;

        public AutofacResolver()
        {
            _container = CreateDefaultContainer();
        }

        public AutofacResolver(IContainer container)
        {
            _container = container;
        }

        public Object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch
            {

                return null;
            }
        }
        public System.Collections.Generic.IEnumerable<System.Object> GetServices(System.Type serviceType)
        {
            try
            {
                return new[] { _container.Resolve(serviceType) };
            }
            catch
            {

                return new object[] { };
            }
        }

        private IContainer CreateDefaultContainer()
        {
            var builder = new ContainerBuilder();
            builder.Register(t => { return WebApiApplication.DocumentStore.OpenSession(); }).As<IDocumentSession>();
            builder.RegisterType<FakeSpeakerRepository>().As<ISpeakerRepository>();
            builder.RegisterType<SpeakerController>();

            return builder.Build();
        }
    }
}
