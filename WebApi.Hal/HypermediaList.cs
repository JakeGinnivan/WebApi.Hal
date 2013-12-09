using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Hal
{
    public class HypermediaList : IList<Link>
    {
        readonly List<Link> innerList;

        public HypermediaList()
        {
            innerList = new List<Link>();
        }

        public IEnumerator<Link> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Link link)
        {
            innerList.Add(link);
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(Link link)
        {
            return innerList.Contains(link);
        }

        public void CopyTo(Link[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(Link link)
        {
            return innerList.Remove(link);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly { get { return false; } }

        public int IndexOf(Link link)
        {
            return innerList.IndexOf(link);
        }

        public void Insert(int index, Link item)
        {
            innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        public Link this[int index]
        {
            get { return innerList[index]; }
            set { innerList[index] = value; }
        }
    }
}