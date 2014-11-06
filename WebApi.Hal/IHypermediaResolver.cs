using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public interface IHypermediaResolver
    {
        /// <summary>
        /// Resolves the link relation for the given resource
        /// </summary>
        /// <param name="resource">Resource to determine the link relation for</param>
        /// <returns>The configured link relation</returns>
        string ResolveRel(IResource resource);

        /// <summary>
        /// Resolves the self link for the given resource
        /// </summary>
        /// <param name="resource">Resource to determine the self link for</param>
        /// <returns>The configured self link</returns>
        Link ResolveSelf(IResource resource);

        /// <summary>
        /// Resolves all non-self and non-CURIES links for the given resource
        /// </summary>
        /// <param name="resource">Resource to determine the links for</param>
        /// <returns>The configured links</returns>
        IEnumerable<Link> ResolveLinks(IResource resource);

        /// <summary>
        /// Resolves the hypermedia appenders for the given resource
        /// </summary>
        /// <typeparam name="T">Type of the resource to find the appender for</typeparam>
        /// <param name="resource">Resource to find the appender for</param>
        /// <returns>Configured appender</returns>
        IHypermediaAppender<T> ResolveAppender<T>(T resource) where T : class, IResource;
    }
}