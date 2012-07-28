using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerStyleListLinker : IResourceLinker<BeerStyleListResource>
    {
        public void CreateLinks(BeerStyleListResource resource, IResourceLinker resourceLinker)
        {
            var beerStyles = LinkTemplates.BeerStyles.GetStyles;
            resource.Href = beerStyles.Href;
            resource.Rel = beerStyles.Rel;

            foreach (var style in resource)
            {
                resourceLinker.CreateLinks(style);
            }
        }
    }
}