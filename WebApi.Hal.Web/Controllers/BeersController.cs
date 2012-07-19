using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Hal.Web.Data;
using System.Linq;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Controllers
{
    public class BeersController : ApiController
    {
        readonly IResourceLinker resourceLinker;
        readonly IBeerContext beerContext;

        public BeersController(IResourceLinker resourceLinker, IBeerContext beerContext)
        {
            this.resourceLinker = resourceLinker;
            this.beerContext = beerContext;
        }

        // GET api/beers
        public BeerListResource Get()
        {
            return GetPage(1);
        }

        public BeerListResource GetPage(int page)
        {
            var beers = beerContext.Beers.Take(20)
                .Select(b=>new BeerResource
                {
                    Id = b.Id,
                    Name = b.Name
                }).ToList();

            var resourceList = new BeerListResource(beers)
            {
                Links =
                {
                    new Link {Href = string.Format("/beers?page={0}", page+1), Rel = "next"}
                }
            };
            return resourceLinker.CreateLinks(resourceList);
        }

        // GET api/beers/5
        public BeerResource Get(int id)
        {
            var beer = beerContext.Beers.Find(id);

            return new BeerResource
            {
                Id = beer.Id,
                Name = beer.Name
            };
        }

        // POST api/beers
        public HttpResponseMessage Post(BeerResource value)
        {
            var newBeer = new Beer(value.Name);
            beerContext.Beers.Add(newBeer);
            beerContext.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Headers =
                {
                    Location = new Uri(string.Format("/beers/{0}", newBeer.Id), UriKind.Relative)
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