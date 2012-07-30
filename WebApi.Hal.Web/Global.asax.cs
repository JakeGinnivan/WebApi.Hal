using System.Configuration;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using WebApi.Hal.Web.App_Start;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        IContainer container;
        string connectionString;

        protected void Application_Start()
        {
            connectionString = ConfigurationManager.AppSettings["BeerDatabase"];

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

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
                .InstancePerHttpRequest();

            containerBuilder
                .RegisterType<BeerRepository>()
                .As<IRepository>()
                .InstancePerHttpRequest();
        }
    }
}