using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Hal
{
    public class ResourceList<T> : HalResource, IEnumerable<T> where T : HalResource
    {
        private readonly List<T> resources;

        public ResourceList()
        {
            resources = new List<T>();
        }

        public ResourceList(List<T> res)
        {
            resources = res ?? new List<T>();
        }

        public T this[int index]
        {
            get
            {
                return resources[index];
            }
        }

        public T this[string name]
        {
            get
            {
                return resources.SingleOrDefault(x => x.LinkName == name);
            }
        }

        /// <summary>
        /// This method was added solely for the purpose of supporting XMLSerializer. 
        /// Exception thrown - To be XML serializable, types which inherit from IEnumerable must have an implementation of Add(System.Object) 
        /// </summary>
        /// <param name="item">Object to be added</param>
        public void Add(T item)
        {
            throw new InvalidOperationException("Cannot add to the list");
        }

        public IEnumerator<T> GetEnumerator()
        {
            return resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}