using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public interface IHypermediaConfiguration
    {
        /// <summary>
        /// Configures the passed in resource by appending all the necesarry hypermedia to its 
        /// <see cref="IResource.Links"/> collection
        /// </summary>
        /// <param name="resource">Resource to configure</param>
        void Configure(IResource resource);
    }
}