using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    internal class EmbeddedResource
    {
        public bool IsSourceAnArray { get; set; }
        public IList<IResource> Resources { get; private set; } = new List<IResource>();
        public string RelationName { get; set; }
    }
}