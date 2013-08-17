using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Api
{
    public class BeerController : ApiController
    {
        readonly IBeerDbContext beerDbContext;

        public BeerController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        // GET api/beers/5
        public BeerRepresentation Get(int id)
        {
            var beer = beerDbContext.Beers.Find(id);

            return new BeerRepresentation
            {
                Id = beer.Id,
                Name = beer.Name,
                BreweryId = beer.Brewery == null ? (int?)null : beer.Brewery.Id,
                BreweryName = beer.Brewery == null ? null : beer.Brewery.Name,
                StyleId = beer.Style == null ? (int?)null : beer.Style.Id,
                StyleName = beer.Style == null ? null : beer.Style.Name
            };
        }

        // POST api/beers
        public HttpResponseMessage Post(BeerRepresentation value)
        {
            var newBeer = new Beer(value.Name);
            beerDbContext.Beers.Add(newBeer);
            beerDbContext.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Headers =
                {
                    Location = LinkTemplates.Beers.Beer.CreateUri(new { id = newBeer.Id })
                }
            };
        }

        // PUT api/beers/5
        public void Put(int id, string value)
        {
        }

        // DELETE api/beers/5
        public void Delete(int id)
        {
        }
    }
}