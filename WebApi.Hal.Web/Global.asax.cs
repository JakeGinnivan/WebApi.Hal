using System.Configuration;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using WebApi.Hal.Web.App_Start;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        IContainer container;
        string connectionString;

        protected void Application_Start()
        {
            connectionString = ConfigurationManager.AppSettings["BeerDatabase"];

            RouteConfig.RegisterRoutes(GlobalConfiguration.Configuration.Routes);

            GlobalConfiguration.Configuration.Formatters.Add(new JsonHalMediaTypeFormatter());
            GlobalConfiguration.Configuration.Formatters.Add(new XmlHalMediaTypeFormatter());

            var containerBuilder = new ContainerBuilder();

            ConfigureContainer(containerBuilder);

            Database.SetInitializer(new DbUpDatabaseInitializer(connectionString));

            container = containerBuilder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            // Register API controllers using assembly scanning.
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            containerBuilder
                .Register(c=> new BeerDbContext(connectionString))
                .As<IBeerDbContext>()
                .InstancePerApiRequest();

            containerBuilder
                .RegisterType<BeerRepository>()
                .As<IRepository>()
                .InstancePerApiRequest();
        }
    }
}