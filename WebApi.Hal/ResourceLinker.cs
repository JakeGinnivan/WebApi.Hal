using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.CSharp;

namespace WebApi.Hal
{
    public class ResourceLinker : IResourceLinker
    {
        readonly Dictionary<Type, object> resourceLinkers = new Dictionary<Type, object>();

        public ResourceLinker() 
        {

        }

        public ResourceLinker(Dictionary<Type, object> resourceLinkers)
        {
            this.resourceLinkers= resourceLinkers;
        }

        public void AddLinker<T>(IResourceLinker<T> resourceLinker)
        {
            var type = typeof(T);
            if (!resourceLinkers.ContainsKey(type))
                resourceLinkers.Add(type, resourceLinker);
        }

        public T CreateLinks<T>(T resource)
        {
            var type = typeof(T);
            if (!resourceLinkers.ContainsKey(type))
                throw new ArgumentException(CreateExceptionMessage(type));

            var linker = (IResourceLinker<T>)resourceLinkers[type];

            linker.CreateLinks(resource, this);
            return resource;
        }

        static string CreateExceptionMessage(Type type)
        {
            return string.Format("No resource linker found for {0}", new CSharpCodeProvider().GetTypeOutput(new CodeTypeReference(type)));
        }
    }
}