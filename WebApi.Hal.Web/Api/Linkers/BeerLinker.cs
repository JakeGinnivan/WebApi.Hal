using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerLinker : IResourceLinker<BeerResource>
    {
        public void CreateLinks(BeerResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = string.Format("/beers/{0}", resource.Id);
            resource.Rel = "beer";
        }
    }
}