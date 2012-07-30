using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;

namespace WebApi.Hal.Web.Api
{
    public class StylesController : ApiController
    {
        readonly IBeerDbContext beerDbContext;
        readonly IRepository repository;

        public StylesController(IBeerDbContext beerDbContext, IRepository repository)
        {
            this.beerDbContext = beerDbContext;
            this.repository = repository;
        }

        public BeerStyleListRepresentation Get()
        {
            var beerStyles = beerDbContext.Styles
                .ToList()
                .Select(s => new BeerStyleRepresentation
                {
                    Id = s.Id,
                    Name = s.Name,
                    Links =
                    {
                        LinkTemplates.BeerStyles.AssociatedBeers.CreateLink(_id=>s.Id)
                    }
                })
                .ToList();

            return new BeerStyleListRepresentation(beerStyles);
        }

        public HttpResponseMessage Get(int id)
        {
            var beerStyle = beerDbContext.Styles.SingleOrDefault(s => s.Id == id);
            if (beerStyle == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var beerStyleResource = new BeerStyleRepresentation
            {
                Id = beerStyle.Id,
                Name = beerStyle.Name,
                Links =
                {
                    LinkTemplates.BeerStyles.AssociatedBeers.CreateLink(_id=>beerStyle.Id)
                }
            };

            return Request.CreateResponse(HttpStatusCode.OK, beerStyleResource);
        }

        [HttpGet]
        public BeerListRepresentation AssociatedBeers(int id)
        {
            return AssociatedBeers(id, 1);
        }

        [HttpGet]
        public BeerListRepresentation AssociatedBeers(int id, int page)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Style.Id == id), page, BeersController.PageSize);

            var resourceList = new BeerListRepresentation(beers.ToList())
            {
                Total = beers.TotalResults
            };

            return resourceList;
        }
    }
}