using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using DbUp;
using WebApi.Hal.Web.Api;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.App_Start;
using WebApi.Hal.Web.Controllers;
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
            resourceLinker.AddLinker(new BeerLinker());
            resourceLinker.AddLinker(new BeerListLinker());
            ConfigureContainer(containerBuilder, resourceLinker);

            EnsureDatabaseUpgraded();

            container = containerBuilder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        void EnsureDatabaseUpgraded()
        {
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(WebApiApplication).Assembly)
                .Build()
                .PerformUpgrade();
        }

        private void ConfigureContainer(ContainerBuilder containerBuilder, ResourceLinker resourceLinker)
        {
            // Register API controllers using assembly scanning.
            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

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

    public class BeerListLinker : IResourceLinker<BeerListResource>
    {
        public void CreateLinks(BeerListResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = "/beers";
            resource.Rel = "beers";

            foreach (var beer in resource)
            {
                resourceLinker.CreateLinks(beer);
            }
        }
    }

    public class BeerLinker : IResourceLinker<BeerResource>
    {
        public void CreateLinks(BeerResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = string.Format("/beers/{0}", resource.Id);
            resource.Rel = "beer";
        }
    }
}