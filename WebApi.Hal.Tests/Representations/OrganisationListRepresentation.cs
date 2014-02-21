using System.Collections.Generic;

namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationListRepresentation : SimpleListRepresentation<OrganisationRepresentation>
    {
        public OrganisationListRepresentation(IList<OrganisationRepresentation> organisationRepresentations) :
            base(organisationRepresentations)
        {
        }

        public override string Rel
        {
            get { return "organisations"; }
            set { }
        }

        public override string Href
        {
            get { return "/api/organisations"; }
            set { }
        }

        protected override void CreateHypermedia()
        {
        }
    }
}