using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerDetailRepresentation : Representation
    {
        public BeerDetailRepresentation()
        {
            Reviews = new List<ReviewRepresentation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public BeerStyleRepresentation Style { get; set; }
        public BreweryRepresentation Brewery { get; set; }
        public List<ReviewRepresentation> Reviews { get; set; }

        public override string Rel
        {
            get { return LinkTemplates.BeerDetails.GetBeerDetail.Rel; }
            set { }
        }

        public override string Href
        {
            get { return LinkTemplates.BeerDetails.GetBeerDetail.CreateLink(new { id = Id }).Href; }
            set { }
        }

        protected override void CreateHypermedia()
        {
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