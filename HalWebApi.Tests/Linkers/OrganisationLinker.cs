using HalWebApi.Tests.Representations;

namespace HalWebApi.Tests.Linkers
{
    public class OrganisationLinker : IResourceLinker<OrganisationRepresentation>
    {
        public void CreateLinks(OrganisationRepresentation resource, IResourceLinker resourceLinker)
        {
            resource.Rel = "organisation";
            resource.Href = string.Format("/api/organisations/{0}", resource.Id);
        }
    }
}