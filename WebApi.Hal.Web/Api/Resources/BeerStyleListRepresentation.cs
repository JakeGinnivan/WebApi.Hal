using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerStyleListRepresentation : SimpleListRepresentation<BeerStyleRepresentation>
    {
        public BeerStyleListRepresentation(IList<BeerStyleRepresentation> beerStyles) : base(beerStyles)
        {
        }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.BeerStyles.GetStyles.Href;

            Links.Add(new Link { Href = Href, Rel = "self" });
        }
    }
}