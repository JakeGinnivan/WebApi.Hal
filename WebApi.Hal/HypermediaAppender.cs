using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public abstract class HypermediaAppender<T> : IHypermediaAppender
        where T : class, IResource
    {
        public void SetResource(IResource resource)
        {
            Resource = resource as T;
        }

        protected T Resource { get; private set; }

        public abstract void Append(IEnumerable<Link> configured);
    }
}