using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerStyleLinker : IResourceLinker<BeerStyleResource>
    {
        public void CreateLinks(BeerStyleResource resource, IResourceLinker resourceLinker)
        {
            var selfLink = LinkTemplates.BeerStyles.Style.CreateLink(id => resource.Id);
            resource.Href = selfLink.Href;
            resource.Rel = selfLink.Rel;
        }
    }
}