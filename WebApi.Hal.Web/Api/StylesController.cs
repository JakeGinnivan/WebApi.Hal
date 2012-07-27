using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    public class StylesController : ApiController
    {
        readonly IBeerContext beerContext;
        readonly IResourceLinker resourceLinker;

        public StylesController(IResourceLinker resourceLinker, IBeerContext beerContext)
        {
            this.resourceLinker = resourceLinker;
            this.beerContext = beerContext;
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
            var beers = beerContext
                .Beers
                .Where(b => b.Style.Id == id)
                .OrderBy(b => b.Name)
                .Select(b => new BeerResource
                {
                    Id = b.Id,
                    Name = b.Name,
                    BreweryId = b.Brewery.Id,
                    BreweryName = b.Brewery.Name,
                    StyleId = b.Style.Id,
                    StyleName = b.Style.Name
                })
                .ToList();

            int count = beerContext.Beers.Count();
            var resourceList = new BeerListResource(beers) {Total = count};

            return resourceLinker.CreateLinks(resourceList);
        }
    }
}