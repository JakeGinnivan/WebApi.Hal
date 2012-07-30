using System.Linq;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    public class BreweriesController : ApiController
    {
        readonly IBeerDbContext beerDbContext;

        public BreweriesController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        public BreweryListRepresentation Get()
        {
            var breweries = beerDbContext.Styles
                .Select(s => new BreweryRepresentation
                {
                    Id = s.Id,
                    Name = s.Name,
                    Links =
                    {
                        new Link{ Href = string.Format("breweries/{0}/beers", s.Id), Rel = "beers"}
                    }
                })
                .ToList();

            return new BreweryListRepresentation(breweries);
        }

        public BreweryRepresentation Get(int id)
        {
            var brewery = beerDbContext.Breweries.Find(id);

            return new BreweryRepresentation
            {
                Id = brewery.Id,
                Name = brewery.Name
            };
        }

        public BeerListRepresentation GetBeers(int id)
        {
            var beers = beerDbContext.Beers
                .Where(b => b.Brewery.Id == id)
                .Select(b=> new BeerRepresentation
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

            return new BeerListRepresentation(beers);
        }
    }
}