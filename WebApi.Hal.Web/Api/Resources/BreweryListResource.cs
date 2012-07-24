using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BreweryListResource : ResourceList<BreweryResource>
    {
        public BreweryListResource(List<BreweryResource> breweries)
            : base(breweries)
        {
        }
    }
}