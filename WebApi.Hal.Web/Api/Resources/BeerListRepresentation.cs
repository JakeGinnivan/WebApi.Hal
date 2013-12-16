using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerListRepresentation : PagedRepresentationList<BeerRepresentation>
    {
        public BeerListRepresentation(IList<BeerRepresentation> beers, int totalResults, int totalPages, int page, Link uriTemplate) :
            base(beers, totalResults, totalPages, page, uriTemplate)
        { }
    }
}