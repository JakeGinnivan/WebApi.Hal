using System.Configuration;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using AutoMapper.Mappers;
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
            var resourceLinker = new ResourceLinker();
            resourceLinker.AddLinkersFromAssembly(typeof(WebApiApplication).Assembly);

            var configurationProvider = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.AllMappers());
            var engine = new MappingEngine(configurationProvider);
            AutomapperMappings.RegisterMaps(configurationProvider);
            ConfigureContainer(containerBuilder, resourceLinker, engine);

            Database.SetInitializer(new DbUpDatabaseInitializer(connectionString));

            container = containerBuilder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private void ConfigureContainer(ContainerBuilder containerBuilder, ResourceLinker resourceLinker, IMappingEngine engine)
        {
            // Register API controllers using assembly scanning.
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            containerBuilder.RegisterInstance(engine);

            containerBuilder
                .RegisterInstance(resourceLinker)
                .As<IResourceLinker>()
                .SingleInstance();

            containerBuilder
                .Register(c=> new BeerDbContext(connectionString))
                .As<IBeerContext>()
                .InstancePerHttpRequest();
        }
    }
}