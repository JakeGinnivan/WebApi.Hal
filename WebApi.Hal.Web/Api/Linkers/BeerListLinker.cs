using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerListLinker : IResourceLinker<BeerListResource>
    {
        public void CreateLinks(BeerListResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = resource.Page == 1 ? "/beers" : string.Format("/beers?page={0}", resource.Page);
            resource.Rel = "beers";

            foreach (var beer in resource)
            {
                resourceLinker.CreateLinks(beer);
            }
        }
    }
}