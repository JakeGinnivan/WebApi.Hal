using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerStyleListResource : ResourceList<BeerStyleResource>
    {
        public BeerStyleListResource(List<BeerStyleResource> beerStyles) : base(beerStyles)
        {
            
        }
    }
}