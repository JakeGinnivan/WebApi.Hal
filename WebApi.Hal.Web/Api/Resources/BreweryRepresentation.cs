namespace WebApi.Hal.Web.Api.Resources
{
    public class BreweryRepresentation : Representation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
            var selfLink = LinkTemplates.Breweries.Brewery.CreateLink(new{Id});
            Href = selfLink.Href;
            Rel = selfLink.Rel;

            Links.Add(new Link { Href = Href, Rel = "self" });
        }
    }
}