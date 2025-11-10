using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public class EmbeddedResource
    {
        public bool IsSourceAnArray { get; set; }
        public IList<IResource> Resources { get; private set; } = new List<IResource>();
        public string RelationName { get; set; }
    }
}