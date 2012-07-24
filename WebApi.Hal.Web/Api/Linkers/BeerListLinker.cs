using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerListLinker : IResourceLinker<BeerListResource>
    {
        public void CreateLinks(BeerListResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = "/beers";
            resource.Rel = "beers";

            foreach (var beer in resource)
            {
                resourceLinker.CreateLinks(beer);
            }
        }
    }
}