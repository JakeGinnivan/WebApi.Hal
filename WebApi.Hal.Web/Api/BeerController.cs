using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/hal+json")]
    public class BeerController : ControllerBase
    {
        readonly IBeerDbContext beerDbContext;

        public BeerController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        // GET beer/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BeerRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BeerRepresentation> Get(int id)
        {
            var beer = beerDbContext.Beers
                                    .Include("Brewery") // lazy loading isn't on for this query; force loading
                                    .Include("Style")
                                    .Single(br => br.Id == id);

            return new BeerRepresentation
            {
                Id = beer.Id,
                Name = beer.Name,
                BreweryId = beer.Brewery?.Id,
                BreweryName = beer.Brewery?.Name,
                StyleId = beer.Style?.Id,
                StyleName = beer.Style?.Name,
                ReviewIds = beerDbContext.Reviews.Where(r => r.Beer_Id == id).Select(r => r.Id).ToList()
            };
        }

        // PUT beer/5 with a hal representation in the body as json. Be sure to set content-type: application/hal+json (and accept: application/hal+json for the response)
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public void Put(int id, [FromBody] BeerRepresentation value)
        {
            Console.WriteLine($"new beer would be updated if repostory supported it! {value.Id}, {value.Name}");
        }

        // DELETE beer?id=1
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public void Delete(int id)
        {
        }
    }
}