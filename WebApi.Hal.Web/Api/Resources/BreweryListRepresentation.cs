using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BreweryListRepresentation : RepresentationList<BreweryRepresentation>
    {
        public BreweryListRepresentation(IList<BreweryRepresentation> breweries)
            : base(breweries)
        {
        }

        protected override void CreateListHypermedia()
        {
            var selfLink = LinkTemplates.Breweries.GetBreweries;
            Href = selfLink.Href;
            Rel = selfLink.Rel;

            Links.Add(new Link { Href = Href, Rel = "self" });
        }
    }
}