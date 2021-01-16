using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

namespace CustomController
{
    public class Startup
    {
        public static void Configuration(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
        }
    }

    public class ServiceRegistration : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            GlobalConfiguration.Configure(Startup.Configuration);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
        }
    }
}
