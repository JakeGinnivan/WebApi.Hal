using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerListRepresentation : RepresentationList<BeerRepresentation>
    {
        public BeerListRepresentation(IList<BeerRepresentation> beers) : base(beers)
        { }

        public int Total { get; set; }
        public int Page { get; set; }

        protected override void CreateHypermedia()
        {
            var selfLink = LinkTemplates.Beers.GetBeers.CreateLink(page => Page);
            Href = Href ?? selfLink.Href;
            Rel = Rel ?? selfLink.Rel;

            Links.Add(new Link("page", LinkTemplates.Beers.GetBeers.Href, true));
            Links.Add(new Link("search", LinkTemplates.Beers.SearchBeers.Href, true));
        }
    }
}