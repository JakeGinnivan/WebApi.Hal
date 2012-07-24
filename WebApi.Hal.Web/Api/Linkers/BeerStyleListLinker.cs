using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerStyleListLinker : IResourceLinker<BeerStyleListResource>
    {
        public void CreateLinks(BeerStyleListResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = "/styles";
            resource.Rel = "styles";

            foreach (var style in resource)
            {
                resourceLinker.CreateLinks(style);
            }
        }
    }
}