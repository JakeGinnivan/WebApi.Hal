using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/hal+json")]
    public class BeersController : ControllerBase
    {
        public const int PageSize = 5;

        readonly IRepository repository;

        public BeersController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET beers
        [HttpGet]
        [ProducesResponseType(typeof(BeerListRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BeerListRepresentation> Get(int page = 1)
        {
            var beers = repository.Find(new GetBeersQuery(), page, PageSize);

            var resourceList = new BeerListRepresentation(beers.ToList(), beers.TotalResults, beers.TotalPages, page, LinkTemplates.Beers.GetBeers);

            return resourceList;
        }

        // GET beers/Search?searchTerm=Roger
        [HttpGet("Search")]
        [ProducesResponseType(typeof(BeerListRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BeerListRepresentation> Search(string searchTerm, int page = 1)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Name.Contains(searchTerm)), page, PageSize);

            // snap page back to actual page found
            if (page > beers.TotalPages) page = beers.TotalPages;

            //var link = LinkTemplates.Beers.SearchBeers.CreateLink(new { searchTerm, page });
            var beersResource = new BeerListRepresentation(beers.ToList(),
                                                           beers.TotalResults,
                                                           beers.TotalPages,
                                                           page,
                                                           LinkTemplates.Beers.SearchBeers,
                                                           new {searchTerm})
                                {
                                    Page = page,
                                    TotalResults = beers.TotalResults
                                };

            return beersResource;
        }

        // POST beers
        [HttpPost]
        [ProducesResponseType(typeof(Beer), (int)HttpStatusCode.Created)]
        public IActionResult Post([FromBody] BeerRepresentation value)
        {
            var newBeer = new Beer
                {Name = value.Name, Style_Id = value.StyleId.Value, Brewery_Id = value.BreweryId.Value};
            repository.Add(newBeer);

            return Created(LinkTemplates.Beers.Beer.CreateUri(new {id = newBeer.Id}), newBeer);
        }
    }
}