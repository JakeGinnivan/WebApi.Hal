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

        // GET beers
        public BeerListRepresentation Get(int page = 1)
        {
            var beers = repository.Find(new GetBeersQuery(), page, PageSize);

            var resourceList = new BeerListRepresentation(beers.ToList(), beers.TotalResults, beers.TotalPages, page, LinkTemplates.Beers.GetBeers);

            return resourceList;
        }

        [HttpGet]
        public BeerListRepresentation Search(string searchTerm, int page = 1)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Name.Contains(searchTerm)), page, PageSize);

            // snap page back to actual page found
            if (page > beers.TotalPages) page = beers.TotalPages;

            //var link = LinkTemplates.Beers.SearchBeers.CreateLink(new { searchTerm, page });
            var beersResource = new BeerListRepresentation(beers.ToList(), beers.TotalResults, beers.TotalPages, page,
                                                           LinkTemplates.Beers.SearchBeers,
                                                           new { searchTerm })
            {
                Page = page,
                TotalResults = beers.TotalResults
            };

            return beersResource;
        }

        
    }
}