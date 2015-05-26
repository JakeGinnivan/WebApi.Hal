using System.Collections.Generic;

namespace WebApi.Hal.Tests.Representations
{
    public class Person : Representation
    {
        public Person(int id, string name, int orgId)
        {
            Id = id;
            Name = name;
            OrganisationId = orgId;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganisationId { get; set; }

        public override string Rel
        {
            get { return "person"; }
            set { }
        }

        public override string Href
        {
            get { return string.Format("/api/organisations/{0}/people/{1}", OrganisationId, Id); }
            set { }
        }

        protected override void CreateHypermedia()
        {
        }
    }

    public class Boss : Person // it's debatable whether some bosses are people
    {
        public Boss(int id, string name, int orgId, bool hasPointHair) : base(id, name, orgId)
        {
            HasPointyHair = hasPointHair;
        }

        public bool HasPointyHair { get; set; }

        public override string Rel
        {
            get { return "boss"; }
            set { }
        }

        public override string Href
        {
            get { return string.Format("/api/organisations/{0}/boss", OrganisationId); }
            set { }
        }

        protected override void CreateHypermedia()
        {
        }
    }

    public class OrganisationWithPeopleDetailRepresentation : Representation
    {
        public OrganisationWithPeopleDetailRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Person> People { get; set; }
        public Boss Boss { get; set; }

        public override string Rel
        {
            get { return "organisation"; }
            set { }
        }

        public override string Href
        {
            get { return string.Format("/api/organisations/{0}", Id); }
            set { }
        }

        protected override void CreateHypermedia()
        {
            var l = new Link
            {
                Rel = "people",
                Href = string.Format("/api/organisations/{0}/people", Id)
            };
            Links.Add(l);
            // intentionally add a duplicate to make sure it gets screened out
            Links.Add(l);
        }
    }
}
