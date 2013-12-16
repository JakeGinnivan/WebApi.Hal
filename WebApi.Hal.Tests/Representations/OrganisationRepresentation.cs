namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationRepresentation : Representation
    {
        public OrganisationRepresentation()
        {
            Rel = "organisation";
        }

        public OrganisationRepresentation(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
            Href = string.Format("/api/organisations/{0}", Id);
        }
    }
}