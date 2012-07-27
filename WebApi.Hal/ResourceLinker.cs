using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
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

        public void AddLinkersFromAssembly(Assembly assembly)
        {
            var linkerTypes = assembly.GetTypes()
                .SelectMany(types => types.GetInterfaces()
                    .Where(interfaces => interfaces.IsGenericType && interfaces.GetGenericTypeDefinition() == typeof(IResourceLinker<>))
                    .Select(interfaces => new { Type = types, Interface = interfaces}))
                    .Where(a=>a.Interface != null);

            foreach (var linkerType in linkerTypes)
            {
                var resourceType = linkerType.Interface.GetGenericArguments()[0];

                var instance = Activator.CreateInstance(linkerType.Type);
                resourceLinkers.Add(resourceType, instance);
            }
        }
    }
}