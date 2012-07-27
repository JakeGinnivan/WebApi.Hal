using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Api
{
    public class BeersController : ApiController
    {
        readonly IResourceLinker resourceLinker;
        readonly IBeerContext beerContext;
        const int PageSize = 5;

        public BeersController(IResourceLinker resourceLinker, IBeerContext beerContext, IMappingEngine mappingEngine)
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
            var beers = beerContext
                .Beers
                .OrderBy(b => b.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(b=> new BeerResource
                {
                    Id = b.Id,
                    Name = b.Name,
                    BreweryId = b.Brewery.Id,
                    BreweryName = b.Brewery.Name,
                    StyleId = b.Style.Id,
                    StyleName = b.Style.Name
                })
                .ToList();

            var count = beerContext.Beers.Count();
            var resourceList = new BeerListResource(beers) { Total = count, Page = page };

            if (page > 1)
                resourceList.Links.Add(new Link {Href = string.Format("/beers?page={0}", page - 1), Rel = "prev"});
            if (count > page * PageSize)
                resourceList.Links.Add(new Link { Href = string.Format("/beers?page={0}", page + 1), Rel = "next" });

            return resourceLinker.CreateLinks(resourceList);
        }

        // GET api/beers/5
        public BeerResource Get(int id)
        {
            var beer = beerContext.Beers.Find(id);

            return resourceLinker.CreateLinks(new BeerResource
            {
                Id = beer.Id,
                Name = beer.Name
            });
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