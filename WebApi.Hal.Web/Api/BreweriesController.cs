using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    public class BreweriesController : Controller
    {
        readonly IBeerDbContext beerDbContext;

        public BreweriesController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        public BreweryListRepresentation Get()
        {
            var breweries = beerDbContext.Styles
                .ToList()
                .Select(s => new BreweryRepresentation
                {
                    Id = s.Id,
                    Name = s.Name
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
    }
}