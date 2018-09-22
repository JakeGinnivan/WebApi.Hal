using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/hal+json")]
    public class BeersFromBreweryController : ControllerBase
    {
        readonly IRepository repository;

        public BeersFromBreweryController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET BeersFromBrewery/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BeerListRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BeerListRepresentation> Get([FromRoute]int id, int page = 1)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Brewery.Id == id), page, BeersController.PageSize);
            return new BeerListRepresentation(beers.ToList(), beers.TotalResults, beers.TotalPages, page, LinkTemplates.Breweries.AssociatedBeers, new {id});
        }
    }
}