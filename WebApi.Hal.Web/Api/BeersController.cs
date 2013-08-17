using System.Linq;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;

namespace WebApi.Hal.Web.Api
{
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