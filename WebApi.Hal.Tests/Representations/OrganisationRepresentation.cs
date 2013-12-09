namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationRepresentation : Representation
    {
        public OrganisationRepresentation()
        {
        }

        public OrganisationRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
            Rel = "organisation";
            Href = string.Format("/api/organisations/{0}", Id);
        }
    }
}