using System.Linq;
using System.Web.Http;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    public class StylesController : ApiController
    {
        readonly IResourceLinker resourceLinker;
        readonly IBeerContext beerContext;

        public StylesController(IResourceLinker resourceLinker, IBeerContext beerContext)
        {
            this.resourceLinker = resourceLinker;
            this.beerContext = beerContext;
        }

        public BeerStyleListResource Get()
        {
            var beerStyles = beerContext.Styles
                .Select(s => new BeerStyleResource
                {
                    Id = s.Id,
                    Name = s.Name,
                    Links =
                    {
                        new Link{ Href = string.Format("styles/{0}/beers", s.Id), Rel = "beers"}
                    }
                })
                .ToList();

            return resourceLinker.CreateLinks(new BeerStyleListResource(beerStyles));
        }
    }
}