using System.Web.Http;
using WebApi.Hal.Web.Api;

namespace WebApi.Hal.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            LinkTemplates.Beers.Beer.RegisterLinkWithWebApi<BeerController>(routes);
            LinkTemplates.Beers.GetBeers.RegisterLinkWithWebApi<BeersController>(routes);
            LinkTemplates.Breweries.Brewery.RegisterLinkWithWebApi<BreweriesController>(routes);
            LinkTemplates.Breweries.GetBreweries.RegisterLinkWithWebApi<BreweriesController>(routes);
            LinkTemplates.Breweries.AssociatedBeers.RegisterLinkWithWebApi<BeersFromBreweryController>(routes);
            LinkTemplates.BeerStyles.Style.RegisterLinkWithWebApi<StylesController>(routes);
            LinkTemplates.BeerStyles.GetStyles.RegisterLinkWithWebApi<StylesController>(routes);
            LinkTemplates.BeerStyles.AssociatedBeers.RegisterLinkWithWebApi<StylesController>(routes);
        }
    }
}