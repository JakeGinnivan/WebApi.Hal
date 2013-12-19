using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerDetailRepresentation : Representation
    {
        public BeerDetailRepresentation()
        {
            Reviews = new List<ReviewRepresentation>();
            Rel = LinkTemplates.BeerDetails.GetBeerDetail.CreateLink(new { id = Id }).Rel;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public BeerStyleRepresentation Style { get; set; }
        public BreweryRepresentation Brewery { get; set; }
        public List<ReviewRepresentation> Reviews { get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.BeerDetails.GetBeerDetail.CreateLink(new { id = Id }).Href;

            Links.Add(new Link { Href = Href, Rel = "self" });

            if (Style != null)
                Links.Add(LinkTemplates.BeerStyles.Style.CreateLink(new { id = Style.Id }));
            if (Brewery != null)
                Links.Add(LinkTemplates.Breweries.Brewery.CreateLink(new { id = Brewery.Id }));
            if (Reviews != null)
                foreach (var rev in Reviews)
                    Links.Add(LinkTemplates.Reviews.GetBeerReview.CreateLink(new { id = rev.Beer_Id, rid = rev.Id }));
        }
    }
}