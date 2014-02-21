namespace WebApi.Hal.Web.Api.Resources
{
    public class BreweryRepresentation : Representation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string Rel
        {
            get { return LinkTemplates.Breweries.Brewery.Rel; }
            set { }
        }

        public override string Href
        {
            get { return LinkTemplates.Breweries.Brewery.CreateLink(new { id = Id }).Href; }
            set { }
        }

        protected override void CreateHypermedia()
        {
            Links.Add(LinkTemplates.Breweries.AssociatedBeers.CreateLink(new { id = Id }));
        }
    }
}