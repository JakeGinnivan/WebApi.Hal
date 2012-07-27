using WebApi.Hal.Interfaces;
using WebApi.Hal.Tests.Representations;

namespace WebApi.Hal.Tests.Linkers
{
    public class OrganisationWithPeopleLinker : IResourceLinker<OrganisationRepresentation>
    {
        public void CreateLinks(OrganisationRepresentation resource, IResourceLinker resourceLinker)
        {
            resource.Rel = "organisation";
            resource.Href = string.Format("/api/organisations/{0}", resource.Id);
            resource.Links.Add(new Link
            {
                Rel = "people",
                Href = string.Format("/api/organisations/{0}/people", resource.Id)
            });
        }
    }
}