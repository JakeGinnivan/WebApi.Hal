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

    public class BeersController : ApiController
    {
        public const int PageSize = 5;

        readonly IRepository repository;

        public BeersController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET api/beers
        public BeerListRepresentation Get(int page)
        {
            var beers = repository.Find(new GetBeersQuery(), page, PageSize);

            var resourceList = new BeerListRepresentation(beers.ToList(), beers.TotalResults, beers.TotalPages, page, LinkTemplates.Beers.GetBeers);

            return resourceList;
        }

        [HttpGet]
        public BeerListRepresentation Search(string searchTerm)
        {
            return Search(searchTerm, 1);
        }

        public BeerListRepresentation Search(string searchTerm, int page)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Name.Contains(searchTerm)), page, PageSize);

            var link = LinkTemplates.Beers.SearchBeers.CreateLink(new{searchTerm, page});
            var beersResource = new BeerListRepresentation(beers.ToList(), beers.TotalResults, beers.TotalPages, page, link)
            {
                Page = 1,
                TotalResults = beers.TotalResults,
                Rel = link.Rel,
                Href = link.Href
            };

            if (page > 1)
                beersResource.Links.Add(LinkTemplates.Beers.SearchBeers.CreateLink("prev", new{searchTerm, page = page - 1}));
            if (page < beers.TotalPages)
                beersResource.Links.Add(LinkTemplates.Beers.SearchBeers.CreateLink("next", new{searchTerm, page = page + 1}));

            return beersResource;
        }

        
    }
}