using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public abstract class SimpleListRepresentation<TResource> : Representation where TResource : IResource
    {
        protected SimpleListRepresentation()
        {
            ResourceList = new List<TResource>();
        }

        protected SimpleListRepresentation(IList<TResource> list)
        {
            ResourceList = list;
        }

        public IList<TResource> ResourceList { get; set; }
    }
}
