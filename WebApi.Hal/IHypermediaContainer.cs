using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public interface IHypermediaContainer : IHypermediaConfiguration
    {
        /// <summary>
        /// Resolves the link relation for the given resource
        /// </summary>
        /// <param name="resource">Resource to determine the link relation for</param>
        /// <returns>The configured link relation</returns>
        string ResolveRel(IResource resource);

        /// <summary>
        /// Resolves the link relation for the given resource type
        /// </summary>
        /// <typeparam name="T">Type of the resource to determine the link relation for</typeparam>
        /// <returns>The configured link relation</returns>
        string ResolveRel<T>() where T : class, IResource;

        /// <summary>
        /// Resolves the self link for the given resource
        /// </summary>
        /// <param name="resource">Resource to determine the self link for</param>
        /// <returns>The configured self link</returns>
        Link ResolveSelf(IResource resource);

        /// <summary>
        /// Resolves the self link for the given resource type
        /// </summary>
        /// <typeparam name="T">Type of the resource to determine the self link for</typeparam>
        /// <returns>The configured self link</returns>
        Link ResolveSelf<T>() where T : class, IResource;

        /// <summary>
        /// Determines the unique CURIES links to based on the provided set of links
        /// </summary>
        /// <param name="links">Links to scan for CURIES links</param>
        /// <returns>Unique CURIES links</returns>
        IEnumerable<Link> ExtractUniqueCuriesLinks(params Link[] links);

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