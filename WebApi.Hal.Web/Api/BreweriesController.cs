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
                        LinkTemplates.Breweries.AssociatedBeers.CreateLink(new { s.Id })
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
                    LinkTemplates.Breweries.AssociatedBeers.CreateLink(new { id })
                }
            };
        }
    }
}