using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerRepresentation : Representation
    {
        public BeerRepresentation()
        {
            Rel = LinkTemplates.Beers.Beer.Rel;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public int? BreweryId { get; set; }
        public string BreweryName { get; set; }

        public int? StyleId { get; set; }
        public string StyleName { get; set; }

        [JsonIgnore]
        public List<int> ReviewIds { get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.Beers.Beer.CreateLink(new {id = Id}).Href;

            Links.Add(new Link{Href = Href, Rel = "self"});

            if (StyleId != null)
                Links.Add(LinkTemplates.BeerStyles.Style.CreateLink(new { id = StyleId }));
            if (BreweryId != null)
                Links.Add(LinkTemplates.Breweries.Brewery.CreateLink(new { id = BreweryId }));

            if (ReviewIds != null && ReviewIds.Count > 0)
                foreach (var rid in ReviewIds)
                    Links.Add(LinkTemplates.Reviews.GetBeerReview.CreateLink(new {id = Id, rid}));
        }
    }
}
