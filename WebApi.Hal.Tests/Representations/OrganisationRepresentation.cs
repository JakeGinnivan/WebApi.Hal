namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationRepresentation : Representation
    {
        static readonly Link NoAppPath = new Link("organisation", "/api/organisations/{0}");

        public OrganisationRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string Rel
        {
            get { return NoAppPath.Rel; }
            set { }
        }

        public override string Href
        {
            get { return string.Format(NoAppPath.Href, Id); }
            set { }
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
        }
    }
}