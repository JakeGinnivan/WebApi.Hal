namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerStyleRepresentation : Representation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
            var selfLink = LinkTemplates.BeerStyles.Style.CreateLink(new {Id});
            Href = selfLink.Href;
            Rel = selfLink.Rel;
        }
    }
}