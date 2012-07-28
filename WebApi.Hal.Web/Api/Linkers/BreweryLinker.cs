using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BreweryLinker : IResourceLinker<BreweryResource>
    {
        public void CreateLinks(BreweryResource resource, IResourceLinker resourceLinker)
        {
            var selfLink = LinkTemplates.Breweries.Brewery.CreateLink(id => resource.Id);
            resource.Href = selfLink.Href;
            resource.Rel = selfLink.Rel;
        }
    }
}