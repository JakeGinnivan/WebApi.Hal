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
    public class BeersFromStyleController : ControllerBase
    {
        readonly IRepository repository;

        public BeersFromStyleController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET BeersFromStyle/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BeerListRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BeerListRepresentation> Get(int id, int page = 1)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Style.Id == id), page, BeersController.PageSize);
            var resourceList = new BeerListRepresentation(
                beers.ToList(),
                beers.TotalResults,
                beers.TotalPages,
                page,
                LinkTemplates.BeerStyles.AssociatedBeers,
                new {id});
            return resourceList;
        }
    }
}