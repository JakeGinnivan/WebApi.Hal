using System.Collections.Generic;

namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationListRepresentation : SimpleListRepresentation<OrganisationRepresentation>
    {
        public OrganisationListRepresentation(IList<OrganisationRepresentation> organisationRepresentations) :
            base(organisationRepresentations)
        {
            Rel = "organisations";
        }

        protected override void CreateHypermedia()
        {
            Href = "/api/organisations";
        }
    }
}