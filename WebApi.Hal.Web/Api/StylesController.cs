using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;

namespace WebApi.Hal.Web.Api
{
    public class StylesController : ApiController
    {
        readonly IBeerContext beerContext;
        readonly IResourceLinker resourceLinker;
        IRepository repository;

        public StylesController(IResourceLinker resourceLinker, IBeerContext beerContext, IRepository repository)
        {
            this.resourceLinker = resourceLinker;
            this.beerContext = beerContext;
            this.repository = repository;
        }

        public BeerStyleListResource Get()
        {
            List<BeerStyleResource> beerStyles = beerContext.Styles
                .ToList()
                .Select(s => new BeerStyleResource
                {
                    Id = s.Id,
                    Name = s.Name,
                    Links =
                    {
                        new Link {Href = string.Format("styles/{0}/beers", s.Id), Rel = "beers"}
                    }
                })
                .ToList();

            return resourceLinker.CreateLinks(new BeerStyleListResource(beerStyles));
        }

        public HttpResponseMessage Get(int id)
        {
            var beerStyle = beerContext.Styles.SingleOrDefault(s => s.Id == id);
            if (beerStyle == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var beerStyleResource = new BeerStyleResource
            {
                Id = beerStyle.Id,
                Name = beerStyle.Name,
                Links =
                {
                    new Link
                    {
                        Href = string.Format("styles/{0}/beers", beerStyle.Id),
                        Rel = "beers"
                    }
                }
            };
            return Request.CreateResponse(HttpStatusCode.OK, resourceLinker.CreateLinks(beerStyleResource));
        }

        [HttpGet]
        public BeerListResource AssociatedBeers(int id)
        {
            return AssociatedBeers(id, 1);
        }

        [HttpGet]
        public BeerListResource AssociatedBeers(int id, int page)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Style.Id == id), page, BeersController.PageSize);

            var resourceList = new BeerListResource(beers.ToList())
            {
                Total = beers.TotalResults
            };

            return resourceLinker.CreateLinks(resourceList);
        }
    }
}