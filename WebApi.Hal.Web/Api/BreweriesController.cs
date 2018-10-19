using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/hal+json")]
    public class BreweriesController : ControllerBase
    {
        readonly IBeerDbContext beerDbContext;

        public BreweriesController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        // GET breweries
        [HttpGet]
        [ProducesResponseType(typeof(BreweryListRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BreweryListRepresentation> Get()
        {
            var breweries = beerDbContext.BeerStyles
                                         .ToList()
                                         .Select(s => new BreweryRepresentation
                                                      {
                                                          Id = s.Id,
                                                          Name = s.Name
                                                      })
                                         .ToList();

            return new BreweryListRepresentation(breweries);
        }

        // GET breweries/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BreweryRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BreweryRepresentation> Get(int id)
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