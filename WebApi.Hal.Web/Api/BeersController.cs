using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Api
{
    public class BeersController : ApiController
    {
        public const int PageSize = 5;

        readonly IBeerDbContext beerDbContext;
        readonly IRepository repository;

        public BeersController(IBeerDbContext beerDbContext, IRepository repository)
        {
            this.beerDbContext = beerDbContext;
            this.repository = repository;
        }

        // GET api/beers
        public BeerListRepresentation Get()
        {
            return GetPage(1);
        }

        public BeerListRepresentation GetPage(int page)
        {
            var beers = repository.Find(new GetBeersQuery(), page, PageSize);

            var resourceList = new BeerListRepresentation(beers.ToList()) { Total = beers.TotalResults, Page = page };

            if (page > 1)
                resourceList.Links.Add(LinkTemplates.Beers.GetBeers.CreateLink("prev", _page => page - 1));
            if (page < beers.TotalPages)
                resourceList.Links.Add(LinkTemplates.Beers.GetBeers.CreateLink("next", _page => page + 1));

            return resourceList;
        }

        [HttpGet]
        public BeerListRepresentation Search(string searchTerm)
        {
            return Search(searchTerm, 1);
        }

        public BeerListRepresentation Search(string searchTerm, int page)
        {
            var beers = repository.Find(new GetBeersQuery(b=>b.Name.Contains(searchTerm)), page, PageSize);

            var link = LinkTemplates.Beers.SearchBeers.CreateLink(_searchTerm => searchTerm, _page => page);
            var beersResource = new BeerListRepresentation(beers.ToList())
            {
                Page = 1,
                Total = beers.TotalResults,
                Rel = link.Rel,
                Href = link.Href
            };

            if (page > 1)
                beersResource.Links.Add(LinkTemplates.Beers.SearchBeers.CreateLink("prev", _searchTerm => searchTerm, _page => page - 1));
            if (page < beers.TotalPages)
                beersResource.Links.Add(LinkTemplates.Beers.SearchBeers.CreateLink("next", _searchTerm => searchTerm, _page => page + 1));

            return beersResource;
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
                    Location = LinkTemplates.Beers.Beer.CreateUri(id => newBeer.Id)
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