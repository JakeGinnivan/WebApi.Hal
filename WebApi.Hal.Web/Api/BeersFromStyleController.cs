using System.Linq;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;

namespace WebApi.Hal.Web.Api
{
    public class BeersFromStyleController : ApiController
    {
        readonly IRepository repository;

        public BeersFromStyleController(IRepository repository)
        {
            this.repository = repository;
        }

        public BeerListRepresentation Get(int id, int page = 1)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Style.Id == id), page, BeersController.PageSize);
            var resourceList = new BeerListRepresentation(
                beers.ToList(), beers.TotalResults, beers.TotalPages, page,
                LinkTemplates.BeerStyles.AssociatedBeers, new {id});
            return resourceList;
        }
    }
}