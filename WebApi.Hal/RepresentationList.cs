using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    [Obsolete("Use SimpleListRepresentation", false)]
    public abstract class RepresentationList<TRepresentation> : Representation, IRepresentationList, IEnumerable<TRepresentation> where TRepresentation : Representation
    {
        private readonly IList<TRepresentation> resources;

        protected RepresentationList(IList<TRepresentation> res)
        {
            resources = res ?? new List<TRepresentation>();
        }

        public TRepresentation this[int index]
        {
            get
            {
                return resources[index];
            }
        }

        public TRepresentation this[string name]
        {
            get
            {
                return resources.SingleOrDefault(x => x.LinkName == name);
            }
        }

        protected internal override void CreateHypermedia()
        {
            CreateListHypermedia();
        }

        protected abstract void CreateListHypermedia();

        /// <summary>
        /// This method was added solely for the purpose of supporting XMLSerializer. 
        /// Exception thrown - To be XML serializable, types which inherit from IEnumerable must have an implementation of Add(System.Object) 
        /// </summary>
        /// <param name="item">Object to be added</param>
        public void Add(TRepresentation item)
        {
            throw new InvalidOperationException("Cannot add to the list");
        }

        public IEnumerator<TRepresentation> GetEnumerator()
        {
            return resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}