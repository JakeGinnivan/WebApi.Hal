using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public interface IHypermediaConfiguration
    {
        void Configure(IResource resource);
        string ResolveRel(IResource resource);
        Link ResolveSelf(IResource resource);
        IEnumerable<Link> ExtractUniqueCuriesLinks(params Link[] links);
        IEnumerable<Link> ResolveLinks(IResource resource);
        IHypermediaAppender<T> ResolveAppender<T>(T resource) where T : class, IResource;
    }
}