using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerDetailListRepresentation : PagedRepresentationList<BeerDetailRepresentation>
    {
        public BeerDetailListRepresentation(IList<BeerDetailRepresentation> beerDetailList, int totalResults, int totalPages, int page, Link uriTemplate) :
            base(beerDetailList, totalResults, totalPages, page, uriTemplate, null)
        {
        }
    }
}