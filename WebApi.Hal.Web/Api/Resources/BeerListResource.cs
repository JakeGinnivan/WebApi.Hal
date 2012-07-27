using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerListResource : ResourceList<BeerResource>
    {
        public BeerListResource(List<BeerResource> beers) : base(beers)
        { }

        public int Total { get; set; }
        public int Page { get; set; }
    }
}