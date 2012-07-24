using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BreweryLinker : IResourceLinker<BreweryResource>
    {
        public void CreateLinks(BreweryResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = string.Format("/breweries/{0}", resource.Id);
            resource.Rel = "brewery";
        }
    }
}