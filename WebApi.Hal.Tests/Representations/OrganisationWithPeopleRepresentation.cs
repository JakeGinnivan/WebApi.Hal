namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationWithPeopleRepresentation : Representation
    {
        public OrganisationWithPeopleRepresentation()
        {
        }

        public OrganisationWithPeopleRepresentation(int id, string name)
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

            Links.Add(new Link
            {
                Rel = "people",
                Href = string.Format("/api/organisations/{0}/people", Id)
            });
            Links.Add(new Link
            {
                Rel = "brownnoser",
                Href = string.Format("/api/organisations/{0}/brown/1", Id)
            });
            Links.Add(new Link
            {
                Rel = "brownnoser",
                Href = string.Format("/api/organisations/{0}/brown/2", Id)
            });
        }
    }
}