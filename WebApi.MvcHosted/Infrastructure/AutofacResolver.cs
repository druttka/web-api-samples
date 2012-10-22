using System;
using Autofac;
using System.Web.Mvc;
using WebApi.Data;
using WebApi.MvcHosted.Api;
using System.Web.Http.Dependencies;
using WebApi.MvcHosted.Controllers;

namespace WebApi.MvcHosted.Infrastructure
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class AutofacResolver : System.Web.Http.Dependencies.IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private readonly IContainer _container;
        private bool _isDisposed = false;

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
            builder.RegisterType<EFSpeakerRepository>().As<ISpeakerRepository>();
            builder.RegisterType<SpeakerController>();
            builder.RegisterType<HomeController>();

            return builder.Build();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
                return;

            if (isDisposing)
            {
                if (_container != null)
                {
                    //_container.Dispose();
                }
            }

            _isDisposed = true;
        }

        ~AutofacResolver()
        {
            this.Dispose(false);
        }
    }
}
