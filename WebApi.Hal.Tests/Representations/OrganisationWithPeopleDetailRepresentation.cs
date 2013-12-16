using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApi.Hal.Tests.Representations
{
    public class Person : Representation
    {
        public Person(int id, string name, int orgId)
        {
            Rel = "person";
            Id = id;
            Name = name;
            OrganisationId = orgId;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganisationId { get; set; }

        protected override void CreateHypermedia()
        {
            Href = string.Format("/api/organisations/{0}/people/{1}", OrganisationId, Id);
        }
    }

    public class Boss : Person // it's debatable whether some bosses are people
    {
        public Boss(int id, string name, int orgId, bool hasPointHair) : base(id, name, orgId)
        {
            Rel = "boss";
            HasPointyHair = hasPointHair;
        }

        public bool HasPointyHair { get; set; }

        protected override void CreateHypermedia()
        {
            Href = string.Format("/api/organisations/{0}/boss", OrganisationId);
        }
    }

    public class OrganisationWithPeopleDetailRepresentation : Representation
    {
        public OrganisationWithPeopleDetailRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
            Rel = "organisation";
            People = new List<Person>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public List<Person> People { get; set; }
        public Boss Boss { get; set; }

        protected override void CreateHypermedia()
        {
            Href = string.Format("/api/organisations/{0}", Id);

            Links.Add(new Link
            {
                Rel = "people",
                Href = string.Format("/api/organisations/{0}/people", Id)
            });
        }
    }
}
