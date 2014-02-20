namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerStyleRepresentation : Representation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string Rel
        {
            get { return LinkTemplates.BeerStyles.Style.Rel; }
            set { }
        }

        public override string Href
        {
            get { return LinkTemplates.BeerStyles.Style.CreateLink(new { id = Id }).Href; }
            set { }
        }

        protected override void CreateHypermedia()
        {
            Links.Add(LinkTemplates.BeerStyles.AssociatedBeers.CreateLink(new {id = Id}));
        }
    }
}