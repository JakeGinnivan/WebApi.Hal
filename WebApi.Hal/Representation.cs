﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal
{
    public abstract class Representation : IResource
    {
        public IList<Link> Links { get; set; } = new List<Link>();

        [JsonProperty("_embedded")]
        private IList<EmbeddedResource> Embedded { get; set; }

        [JsonIgnore]
        private readonly IDictionary<PropertyInfo, object> embeddedResourceProperties = new Dictionary<PropertyInfo, object>();

        [JsonIgnore]
        public HalJsonConverterContext ConverterContext { get; set; }

        [OnSerializing]
        private void OnSerialize(StreamingContext context)
        {
            // Clear the embeddedResourceProperties in order to make this object re-serializable.
            embeddedResourceProperties.Clear();

            if (ConverterContext == null)
                return;

            var ctx = ConverterContext;
            if (!ctx.IsRoot)
                return;

            var curies = new List<CuriesLink>();

            RepopulateRecursively(ctx.HypermediaResolver, curies);

            if (Links != null)
                Links = curies
                    .Distinct(CuriesLink.EqualityComparer)
                    .Select(x => x.ToLink())
                    .Union(Links)
                    .ToList();

            ctx.IsRoot = false;
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            if (ConverterContext != null)
            {
                // restore embedded resource properties
                foreach (var prop in embeddedResourceProperties.Keys)
                    prop.SetValue(this, embeddedResourceProperties[prop], null);
            }
        }

        private Link ToLink(IHypermediaResolver resolver)
        {
            Link link = null;

            if (resolver != null)
            {
                link = resolver.ResolveSelf(this);

                if (link != null)
                {
                    link = link.Clone();
                    link.Rel = resolver.ResolveRel(this);
                }
            }

            if ((resolver == null) || (link == null))
            {
                link = Links.SingleOrDefault(x => x.Rel == "self");

                if (link != null)
                {
                    link = link.Clone();
                    link.Rel = Rel;
                }
            }

            return link;
        }

        private void RepopulateRecursively(IHypermediaResolver resolver, List<CuriesLink> curies)
        {
            var type = GetType();

            if (resolver == null)
                RepopulateHyperMedia();
            else
                ResolveAndAppend(resolver, type);

            // put all embedded resources and lists of resources into Embedded for the _embedded serializer
            Embedded = new List<EmbeddedResource>();

            foreach (var property in type.GetProperties().Where(p => IsEmbeddedResourceType(p.PropertyType)))
            {
                var value = property.GetValue(this, null);

                if (value == null)
                    continue; // nothing to serialize for this property ...

                // remember embedded resource property for restoring after serialization
                embeddedResourceProperties.Add(property, value);

                if (value is IResource resource)
                    ProcessPropertyValue(resolver, curies, resource);
                else
                    ProcessPropertyValue(resolver, curies, (IEnumerable<IResource>)value);

                // null out the embedded property so it doesn't serialize separately as a property
                property.SetValue(this, null, null);
            }

            if (Links != null)
                curies.AddRange(Links.Where(l => l.Curie != null).Select(l => l.Curie));

            if (Embedded.Count == 0)
                Embedded = null; // avoid the property from being serialized ...
        }

        private void ProcessPropertyValue(IHypermediaResolver resolver, List<CuriesLink> curies, IEnumerable<IResource> resources)
        {
            var resourceList = resources.ToList();

            var relationName = resources is IResourceList list ? list.RelationName : resourceList.FirstOrDefault()?.Rel ?? string.Empty;

            var embeddedResource = new EmbeddedResource {IsSourceAnArray = true, RelationName = relationName};

            foreach (var resourceItem in resourceList)
            {
                embeddedResource.Resources.Add(resourceItem);

                if (!(resourceItem is Representation representation))
                    continue;

                representation.RepopulateRecursively(resolver, curies); // traverse ...
                var link = representation.ToLink(resolver);

                if (link != null)
                {
                    link.IsMultiLink = true;
                    Links.Add(link); // add a link to embedded to the container ...
                }
            }

            Embedded.Add(embeddedResource);
        }

        private void ProcessPropertyValue(IHypermediaResolver resolver, List<CuriesLink> curies, IResource resource)
        {
            var embeddedResource = new EmbeddedResource {IsSourceAnArray = false, RelationName = resource.Rel};
            embeddedResource.Resources.Add(resource);

            Embedded.Add(embeddedResource);

            if (!(resource is Representation representation))
                return;

            representation.RepopulateRecursively(resolver, curies); // traverse ...
            var link = representation.ToLink(resolver);

            if (link != null)
                Links.Add(link); // add a link to embedded to the container ...
        }

        private void ResolveAndAppend(IHypermediaResolver resolver, Type type)
        {
            // We need reflection here, because appenders are of type IHypermediaAppender<T> whilst we define this logic in the base class of T

            var methodInfo = type.GetMethod("Append", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.NonPublic);
            var genericMethod = methodInfo.MakeGenericMethod(type);

            genericMethod.Invoke(this, new object[] {this, resolver});
        }

        protected static void Append<T>(IResource resource, IHypermediaResolver resolver) where T : class, IResource
            // called using reflection ...
        {
            var typed = resource as T;

            if (typed == null)
            {
                throw new ArgumentOutOfRangeException(nameof(resource), "resource must be of type " + typeof(T));
            }

            IHypermediaAppender<T> appender = resolver.ResolveAppender(typed);
            List<Link> configured = resolver.ResolveLinks(typed).ToList();
            Link link = resolver.ResolveSelf(typed);

            if (link != null)
                configured.Insert(0, link);

            if (configured.Count > 0 && (appender != null))
            {
                if (typed.Links == null)
                    typed.Links = new List<Link>(); // make sure resource.Links.Add() can safely be called inside the appender

                appender.Append(typed, configured);

                if ((typed.Links != null) && !typed.Links.Any())
                    typed.Links = null; // prevent _links property serialization
            }
        }

        internal static bool IsEmbeddedResourceType(Type type)
        {
            return typeof (IResource).IsAssignableFrom(type) ||
                   typeof (IEnumerable<IResource>).IsAssignableFrom(type);
        }

        public void RepopulateHyperMedia()
        {
            if (Links == null)
                Links = new List<Link>(); // make sure resource.Links.Add() can safely be called inside the overload

            CreateHypermedia();

            if (!string.IsNullOrEmpty(Href) && Links.Count(l=>l.Rel == "self") == 0)
                Links.Insert(0, new Link { Rel = "self", Href = Href });

            if ((Links != null) && !Links.Any())
                Links = null; // prevent _links property serialization
        }

        [JsonIgnore]
        public virtual string Rel { get; set; }

        [JsonIgnore]
        public virtual string Href { get; set; }

        [JsonIgnore]
        public string LinkName { get; set; }

        protected internal virtual void CreateHypermedia()
        {
        }
    }
}
