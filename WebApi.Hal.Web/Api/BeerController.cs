using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    public class BeerController : Controller
    {
        readonly IBeerDbContext beerDbContext;

        public BeerController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        [HttpGet("{id}")]
        // GET beer/5
        public BeerRepresentation Get(int id)
        {
            var beer = beerDbContext.Beers
                                    .Include("Brewery") // lazy loading isn't on for this query; force loading
                                    .Include("BeerStyle")
                                    .Single(br => br.Id == id);

            return new BeerRepresentation
                   {
                       Id = beer.Id,
                       Name = beer.Name,
                       BreweryId = beer.Brewery == null ? (int?) null : beer.Brewery.Id,
                       BreweryName = beer.Brewery == null ? null : beer.Brewery.Name,
                       StyleId = beer.Style == null ? (int?) null : beer.Style.Id,
                       StyleName = beer.Style == null ? null : beer.Style.Name,
                       ReviewIds = beerDbContext.Reviews.Where(r => r.Beer_Id == id).Select(r => r.Id).ToList()
                   };
        }

        [HttpPut]
        // PUT beer?id=1&value=foo
        public void Put(int id, string value)
        {
        }

        [HttpDelete]
        // DELETE beer?id=1
        public void Delete(int id)
        {
        }
    }
}