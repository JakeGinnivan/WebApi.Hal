using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BreweryListLinker : IResourceLinker<BreweryListResource>
    {
        public void CreateLinks(BreweryListResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = "/breweries";
            resource.Rel = "brewery";

            foreach (var brewery in resource)
            {
                resourceLinker.CreateLinks(brewery);
            }
        }
    }
}