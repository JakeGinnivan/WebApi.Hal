namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerStyleRepresentation : Representation
    {
        public BeerStyleRepresentation()
        {
            Rel = LinkTemplates.BeerStyles.Style.CreateLink(new { Id }).Rel;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.BeerStyles.Style.CreateLink(new { Id }).Href;

            Links.Add(new Link { Href = Href, Rel = "self" });
        }
    }
}