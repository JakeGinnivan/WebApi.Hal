using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CSharp;

namespace HalWebApi
{
    public class ResourceLinker : IResourceLinker
    {
        readonly Dictionary<Type, object> resourceLinkers = new Dictionary<Type, object>();

        public void AddLinker<T>(IResourceLinker<T> resourceLinker)
        {
            var type = typeof(T);
            if (!resourceLinkers.ContainsKey(type))
                resourceLinkers.Add(type, resourceLinker);
        }

        public void CreateLinks<T>(T resource)
        {
            var type = typeof(T);
            if (!resourceLinkers.ContainsKey(type))
                throw new ArgumentException(CreateExceptionMessage(type));

            var linker = (IResourceLinker<T>)resourceLinkers[type];

            linker.CreateLinks(resource, this);
        }

        static string CreateExceptionMessage(Type type)
        {
            return string.Format("No resource linker found for {0}", new CSharpCodeProvider().GetTypeOutput(new CodeTypeReference(type)));
        }
    }
}