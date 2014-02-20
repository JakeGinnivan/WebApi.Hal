namespace WebApi.Hal.Web.Api.Resources
{
    public class ReviewRepresentation : Representation
    {
        public int Id { get; set; }
        public int Beer_Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public override string Rel
        {
            get { return LinkTemplates.Reviews.GetBeerReview.Rel; }
            set { }
        }

        public override string Href
        {
            get { return LinkTemplates.Reviews.GetBeerReview.CreateLink(new { id = Beer_Id, rid = Id }).Href; }
            set { }
        }

        protected override void CreateHypermedia()
        {
            Links.Add(LinkTemplates.Beers.Beer.CreateLink(new {id = Beer_Id}));
        }
    }
}