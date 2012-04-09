using System;
using Autofac;

namespace WebApi.MvcHosted.Infrastructure
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class AutofacResolver : System.Web.Http.Services.IDependencyResolver
    {
        private readonly IContainer _container;
        public AutofacResolver(IContainer container)
        {
            _container = container;
        }
        public System.Object GetService(System.Type serviceType)
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
    }
}
