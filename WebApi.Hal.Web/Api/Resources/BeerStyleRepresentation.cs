namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerStyleRepresentation : Representation
    {
        public BeerStyleRepresentation()
        {
            Rel = LinkTemplates.BeerStyles.Style.Rel;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.BeerStyles.Style.CreateLink(new {id = Id}).Href;

            Links.Add(new Link { Href = Href, Rel = "self" });
        }
    }
}