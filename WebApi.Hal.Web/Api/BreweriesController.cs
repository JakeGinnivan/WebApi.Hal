using System.Linq;
using System.Web.Http;
using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    public class BreweriesController : ApiController
    {
        readonly IResourceLinker resourceLinker;
        readonly IBeerDbContext beerDbContext;

        public BreweriesController(IResourceLinker resourceLinker, IBeerDbContext beerDbContext)
        {
            this.resourceLinker = resourceLinker;
            this.beerDbContext = beerDbContext;
        }

        public BreweryListResource Get()
        {
            var breweries = beerDbContext.Styles
                .Select(s => new BreweryResource
                {
                    Id = s.Id,
                    Name = s.Name,
                    Links =
                    {
                        new Link{ Href = string.Format("breweries/{0}/beers", s.Id), Rel = "beers"}
                    }
                })
                .ToList();

            return resourceLinker.CreateLinks(new BreweryListResource(breweries));
        }

        public BreweryResource Get(int id)
        {
            var brewery = beerDbContext.Breweries.Find(id);

            return resourceLinker.CreateLinks(new BreweryResource
            {
                Id = brewery.Id,
                Name = brewery.Name
            });
        }

        public BeerListResource GetBeers(int id)
        {
            var beers = beerDbContext.Beers
                .Where(b => b.Brewery.Id == id)
                .Select(b=> new BeerResource
                {
                    Id = b.Id,
                    Name = b.Name,
                    StyleName = b.Style.Name,
                    BreweryName = b.Brewery.Name,
                    Links =
                    {
                        new Link{Rel = "style", Href = string.Format("/styles/{0}", b.Style.Id )},
                        new Link{Rel = "brewery", Href = string.Format("/breweries/{0}", b.Style.Id )}
                    }
                }).ToList();

            return resourceLinker.CreateLinks(new BeerListResource(beers));
        }
    }
}