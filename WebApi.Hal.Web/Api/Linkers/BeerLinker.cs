using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerLinker : IResourceLinker<BeerResource>
    {
        public void CreateLinks(BeerResource resource, IResourceLinker resourceLinker)
        {
            resource.Href = string.Format("/beers/{0}", resource.Id);
            resource.Rel = "beer";

            resource.Links.Add(new Link { Rel = "style", Href = string.Format("/styles/{0}", resource.StyleId) });
            resource.Links.Add(new Link { Rel = "brewery", Href = string.Format("/brewery/{0}", resource.BreweryId) });
        }
    }
}