using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public class ResourceList<TResource> : List<TResource>, IResourceList where TResource : IResource
    {
        public ResourceList(string relationName) : this(relationName, new TResource[0])
        {
        }

        public ResourceList(string relationName, IEnumerable<TResource> resources) : base(resources)
        {
            RelationName = relationName;
        }

        public string RelationName { get; }
    }
}