using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BreweryListLinker : IResourceLinker<BreweryListResource>
    {
        public void CreateLinks(BreweryListResource resource, IResourceLinker resourceLinker)
        {
            var selfLink = LinkTemplates.Breweries.GetBreweries;
            resource.Href = selfLink.Href;
            resource.Rel = selfLink.Rel;

            foreach (var brewery in resource)
            {
                resourceLinker.CreateLinks(brewery);
            }
        }
    }
}