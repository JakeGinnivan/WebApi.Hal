namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationRepresentation : Resource
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
    }
}