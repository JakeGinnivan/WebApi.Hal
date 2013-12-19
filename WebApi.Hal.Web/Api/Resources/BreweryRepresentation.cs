namespace WebApi.Hal.Web.Api.Resources
{
    public class BreweryRepresentation : Representation
    {
        public BreweryRepresentation()
        {
            Rel = LinkTemplates.Breweries.Brewery.CreateLink(new { Id }).Rel;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.Breweries.Brewery.CreateLink(new { Id }).Href;

            Links.Add(new Link { Href = Href, Rel = "self" });
        }
    }
}