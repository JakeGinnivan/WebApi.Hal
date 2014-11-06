using System.Collections.Generic;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    internal class EmbeddedResource
    {
        public EmbeddedResource()
        {
            Resources = new List<IResource>();
        }

        public bool IsSourceAnArray { get; set; }
        public IList<IResource> Resources { get; private set; }
    }
}