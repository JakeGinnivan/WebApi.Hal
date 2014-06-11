using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public interface IHypermediaAppender
    {
        void Append(IEnumerable<Link> configured);
        void SetResource(IResource resource);
    }
}