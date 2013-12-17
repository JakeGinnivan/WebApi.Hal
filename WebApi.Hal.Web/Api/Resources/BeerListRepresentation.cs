using System.Collections.Generic;
using System.Linq;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerListRepresentation : PagedRepresentationList<BeerRepresentation>
    {
        public BeerListRepresentation(IList<BeerRepresentation> beers, int totalResults, int totalPages, int page, Link uriTemplate) :
            base(beers, totalResults, totalPages, page, uriTemplate, null)
        { }
        public BeerListRepresentation(IList<BeerRepresentation> beers, int totalResults, int totalPages, int page, Link uriTemplate, object uriTemplateSubstitutionParams) :
            base(beers, totalResults, totalPages, page, uriTemplate, uriTemplateSubstitutionParams)
        { }

        protected override void CreateHypermedia()
        {
            base.CreateHypermedia();
            var search = LinkTemplates.Beers.SearchBeers;
            if (Links.Count(l=>l.Rel == search.Rel && l.Href == search.Href) == 0)
                Links.Add(LinkTemplates.Beers.SearchBeers);
        }
    }
}