using System.Collections.Generic;

namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationListRepresentation : RepresentationList<OrganisationRepresentation>
    {
        public OrganisationListRepresentation(IList<OrganisationRepresentation> organisationRepresentations) :
            base(organisationRepresentations)
        {
            
        }

        protected override void CreateListHypermedia()
        {
            Rel = "organisations";
            Href = "/api/organisations";
        }
    }
}