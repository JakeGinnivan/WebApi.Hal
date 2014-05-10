using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal.Proposed
{
    public interface IHypermediaAppender
    {
        void Append(IEnumerable<Link> configured);
        void SetResource(IResource resource);
    }
}