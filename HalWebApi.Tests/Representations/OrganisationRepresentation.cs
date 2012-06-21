namespace HalWebApi.Tests.Representations
{
    public class OrganisationRepresentation : HalResource
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