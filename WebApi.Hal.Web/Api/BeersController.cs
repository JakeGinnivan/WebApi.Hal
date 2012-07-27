using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Api
{
    public class BeersController : ApiController
    {
        readonly IResourceLinker resourceLinker;
        readonly IBeerContext beerContext;
        readonly IRepository repository;
        public const int PageSize = 5;

        public BeersController(IResourceLinker resourceLinker, IBeerContext beerContext, IRepository repository)
        {
            this.resourceLinker = resourceLinker;
            this.beerContext = beerContext;
            this.repository = repository;
        }

        // GET api/beers
        public BeerListResource Get()
        {
            return GetPage(1);
        }

        public BeerListResource GetPage(int page)
        {
            var beers = repository.Find(new GetBeersQuery(), page, PageSize);

            var resourceList = new BeerListResource(beers.ToList()) { Total = beers.TotalResults, Page = page };

            if (page > 1)
                resourceList.Links.Add(new Link {Href = string.Format("/beers?page={0}", page - 1), Rel = "prev"});
            if (page < beers.TotalPages)
                resourceList.Links.Add(new Link { Href = string.Format("/beers?page={0}", page + 1), Rel = "next" });

            return resourceLinker.CreateLinks(resourceList);
        }

        [HttpGet]
        public BeerListResource Search(string searchTerm)
        {
            return Search(searchTerm, 1);
        }

        public BeerListResource Search(string searchTerm, int page)
        {
            var beers = repository.Find(new GetBeersQuery(b=>b.Name.Contains(searchTerm)), page, PageSize);

            var encodedSearch = HttpUtility.UrlEncode(searchTerm);
            var beersResource = new BeerListResource(beers.ToList())
            {
                Page = 1,
                Total = beers.TotalResults,
                Rel = "beerSearch",
                Href = string.Format(page == 1 ? "/beers?searchTerm={0}" : "/beers?searchTerm={0}&page={1}", encodedSearch, page)
            };

            if (page > 1)
                beersResource.Links.Add(new Link { Href = string.Format("/beers?searchTerm={0}&page={1}", encodedSearch, page - 1), Rel = "prev" });
            if (page < beers.TotalPages)
                beersResource.Links.Add(new Link { Href = string.Format("/beers?searchTerm={0}&page={1}", encodedSearch, page + 1), Rel = "next" });

            return resourceLinker.CreateLinks(beersResource);
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