using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerStyleLinker : IResourceLinker<BeerStyleResource>
    {
        public void CreateLinks(BeerStyleResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = string.Format("/styles/{0}", resource.Id);
            resource.Rel = "style";
        }
    }
}