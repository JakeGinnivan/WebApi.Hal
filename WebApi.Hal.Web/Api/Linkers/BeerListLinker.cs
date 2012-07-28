using WebApi.Hal.Interfaces;
using WebApi.Hal.Web.Api.Resources;

namespace WebApi.Hal.Web.Api.Linkers
{
    public class BeerListLinker : IResourceLinker<BeerListResource>
    {
        public void CreateLinks(BeerListResource resource, IResourceLinker resourceLinker)
        {
            var selfLink = LinkTemplates.Beers.GetBeers.CreateLink(page => resource.Page);
            resource.Href = resource.Href ?? selfLink.Href;
            resource.Rel = resource.Rel ?? selfLink.Rel;

            resource.Links.Add(new Link("page", LinkTemplates.Beers.GetBeers.Href));
            resource.Links.Add(new Link("search", LinkTemplates.Beers.SearchBeers.Href));

            foreach (var beer in resource)
            {
                resourceLinker.CreateLinks(beer);
            }
        }
    }
}