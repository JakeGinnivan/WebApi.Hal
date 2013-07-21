using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerStyleListRepresentation : RepresentationList<BeerStyleRepresentation>
    {
        public BeerStyleListRepresentation(IList<BeerStyleRepresentation> beerStyles) : base(beerStyles)
        {
            
        }

        protected override void CreateListHypermedia()
        {
            var beerStyles = LinkTemplates.BeerStyles.GetStyles;
            Href = beerStyles.Href;
            Rel = beerStyles.Rel;
        }
    }
}