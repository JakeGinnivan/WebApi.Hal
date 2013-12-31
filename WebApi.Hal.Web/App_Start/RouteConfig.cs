using System.Web.Http;
using WebApi.Hal.Web.Api;

namespace WebApi.Hal.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute("BeersRoute", "beers/{id}", new { controller = "Beer" }); // this one is needed only because beers vs. 1 beer are separated into 2 controllers
            routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });
            routes.MapHttpRoute("BreweryBeersRoute", "breweries/{id}/beers", new { controller = "BeersFromBrewery" });
            routes.MapHttpRoute("StyleBeersRoute", "styles/{id}/beers", new { controller = "BeersFromStyle" });
        }
    }
}