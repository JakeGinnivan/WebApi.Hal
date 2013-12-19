using System.Web.Http;
using WebApi.Hal.Web.Api;

namespace WebApi.Hal.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
#if false // RegisterLinkWithWebApi is obsolete because the URI Template syntax it expects is not consistent with RFC6570 used by HAL spec
            LinkTemplates.Beers.Beer.RegisterLinkWithWebApi<BeerController>(routes);
            LinkTemplates.Beers.GetBeers.RegisterLinkWithWebApi<BeersController>(routes);
            LinkTemplates.Beers.SearchBeers.RegisterLinkWithWebApi<BeersController>(routes);

            LinkTemplates.Breweries.Brewery.RegisterLinkWithWebApi<BreweriesController>(routes);
            LinkTemplates.Breweries.GetBreweries.RegisterLinkWithWebApi<BreweriesController>(routes);
            LinkTemplates.Breweries.AssociatedBeers.RegisterLinkWithWebApi<BeersFromBreweryController>(routes);

            LinkTemplates.BeerStyles.Style.RegisterLinkWithWebApi<StylesController>(routes);
            LinkTemplates.BeerStyles.GetStyles.RegisterLinkWithWebApi<StylesController>(routes);
            LinkTemplates.BeerStyles.AssociatedBeers.RegisterLinkWithWebApi<StylesController>(routes);

            LinkTemplates.BeerDetails.GetBeerDetail.RegisterLinkWithWebApi<BeerDetailController>(routes);
#else
            routes.MapHttpRoute("BeersRoute", "beers/{id}", new { controller = "Beer" }); // this one is needed only because beers vs. 1 beer are separated into 2 controllers
            routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });
            routes.MapHttpRoute("BreweryBeersRoute", "breweries/{id}/beers", new { controller = "BeersFromBrewery" });
            routes.MapHttpRoute("StyleBeersRoute", "styles/{id}/beers", new { controller = "BeersFromStyle" });
#endif
        }
    }
}