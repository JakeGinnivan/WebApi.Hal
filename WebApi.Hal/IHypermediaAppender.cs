using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public interface IHypermediaAppender<T> where T:class, IResource
    {
        void Append(T resource, IEnumerable<Link> configured);
    }
}