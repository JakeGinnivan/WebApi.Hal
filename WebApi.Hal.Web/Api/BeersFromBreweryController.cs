using System.Linq;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;

namespace WebApi.Hal.Web.Api
{
    public class BeersFromBreweryController : ApiController
    {
        readonly IRepository repository;

        public BeersFromBreweryController(IRepository repository)
        {
            this.repository = repository;
        }

        public BeerListRepresentation Get(int id, int page)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Brewery.Id == id), page, BeersController.PageSize);
            var link = LinkTemplates.Breweries.AssociatedBeers.CreateLink(new { id });
            return new BeerListRepresentation(beers.ToList(), beers.TotalResults, beers.TotalPages, page, link);
        }
    }
}