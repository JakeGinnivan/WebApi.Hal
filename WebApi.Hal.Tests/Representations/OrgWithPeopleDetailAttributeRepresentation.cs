using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Hal.Tests.Representations
{
    [Self("orgs", "~/orgs/{id}")]
    [Link("employees", "~/orgs/management/{orgId}/{bossId}", "Employees under Boss", "orgId:Id,bossId:Boss.Id")]
    class OrgWithPeopleDetailAttributeRepresentation : AttributeRepresentation
    {
        Manager boss = new Manager(0, null, 0, false);
        List<Employee> people = new List<Employee>();

        public OrgWithPeopleDetailAttributeRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        [HrefKey]
        public int Id { get; set; }
        public string Name { get; set; }

        [Link("employees", "~/orgs/employees/{id}", "Employees in this Org")]
        [Rel("people")]
        public List<Employee> People
        {
            get { return people; }
            set { people = value; }
        }

        [Link("boss", "~/orgs/management/{id}", "The Boss for this Org")]
        [Rel("boss")]
        public Manager Boss
        {
            get { return boss; }
            set { boss = value; }
        }
    }

    public class Employee : AttributeRepresentation
    {
        public Employee(int id, string name, int orgId)
        {
            Id = id;
            Name = name;
            OrganisationId = orgId;
        }

        [HrefKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganisationId { get; set; }
    }

    public class Manager : Employee // it's debatable whether some bosses are people
    {
        public Manager(int id, string name, int orgId, bool hasPointHair)
            : base(id, name, orgId)
        {
            HasPointyHair = hasPointHair;
        }

        public bool HasPointyHair { get; set; }
    }

}
