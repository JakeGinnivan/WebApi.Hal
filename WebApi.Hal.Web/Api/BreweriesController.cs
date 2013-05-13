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
                        LinkTemplates.Breweries.AssociatedBeers.CreateLink(id => s.Id)
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
                Name = brewery.Name,
                Links =
                {
                    LinkTemplates.Breweries.AssociatedBeers.CreateLink(_id => id)
                }
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
                        LinkTemplates.BeerStyles.Style.CreateLink(_id => b.Style.Id),
                        LinkTemplates.Breweries.Brewery.CreateLink(_id => b.Brewery.Id)
                    }
                }).ToList();

            return new BeerListRepresentation(beers);
        }
    }
}