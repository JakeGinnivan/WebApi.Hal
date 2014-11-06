using System;
using System.Collections;
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
        protected Representation()
        {
            Links = new List<Link>();
        }

        [JsonProperty("_embedded")]
        private IList<EmbeddedResource> Embedded { get; set; }

        [JsonIgnore]
        readonly IDictionary<PropertyInfo, object> embeddedResourceProperties = new Dictionary<PropertyInfo, object>();

        [OnSerializing]
        private void OnSerialize(StreamingContext context)
        {
            // Clear the embeddedResourceProperties in order to make this object re-serializable.
            embeddedResourceProperties.Clear();

            if (!ResourceConverter.IsResourceConverterContext(context))
                return;
            
            var ctx = (HalJsonConverterContext)context.Context;

            if (!ctx.IsRoot) 
                return;
            
            var curies = new List<CuriesLink>();
            
            RepopulateRecursively(ctx.HypermediaResolver, curies);

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
            if (ResourceConverter.IsResourceConverterContext(context))
            {
                // restore embedded resource properties
                foreach (var prop in embeddedResourceProperties.Keys)
                    prop.SetValue(this, embeddedResourceProperties[prop], null);
            }
        }

        Link ToLink(IHypermediaResolver resolver)
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
            {
                CreateHypermedia();

                if (!string.IsNullOrEmpty(Href) && Links.Count(l => l.Rel == "self") == 0)
                    Links.Insert(0, new Link {Rel = "self", Href = Href});
            }
            else
            {
                ResolveAndAppend(resolver, type);
            }

            // put all embedded resources and lists of resources into Embedded for the _embedded serializer
            Embedded = new List<EmbeddedResource>();

            foreach (var prop in type.GetProperties().Where(p => IsEmbeddedResourceType(p.PropertyType)))
            {
                var value = prop.GetValue(this, null);
                
                if (value == null) 
                    continue; // nothing to serialize for this property ...

                // remember embedded resource property for restoring after serialization
                embeddedResourceProperties.Add(prop, value);

                // add embedded resource to collection for the serializtion
                var resource = value as IResource;
                var embeddedResource = new EmbeddedResource();

                if (resource != null)
                {
                    embeddedResource.IsSourceAnArray = false;
                    embeddedResource.Resources.Add(resource);

                    Embedded.Add(embeddedResource);

                    var representation = resource as Representation;

                    if (representation != null)
                    {
                        representation.RepopulateRecursively(resolver, curies); // traverse ...
                        Links.Add(representation.ToLink(resolver)); // add a link to embedded to the container ...
                    }
                }
                else
                {
                    var resources = (IEnumerable<IResource>) value;
                    var resourceList = resources.ToList();

                    if (resourceList.Any())
                    {
                        embeddedResource.IsSourceAnArray = true;

                        foreach (var resourceItem in resourceList)
                        {
                            embeddedResource.Resources.Add(resourceItem);

                            var representation = resourceItem as Representation;

                            if (representation == null)
                                continue;

                            representation.RepopulateRecursively(resolver, curies); // traverse ...
                            Links.Add(representation.ToLink(resolver)); // add a link to embedded to the container ...
                        }

                        Embedded.Add(embeddedResource);
                    }
                }

                // null out the embedded property so it doesn't serialize separately as a property
                prop.SetValue(this, null, null);
            }

            curies.AddRange(Links.Where(l => l.Curie != null).Select(l => l.Curie));

            if (Embedded.Count == 0)
                Embedded = null; // avoid the property from being serialized ...
        }

        void ResolveAndAppend(IHypermediaResolver resolver, Type type)
        {
            // We need reflection here, because appenders are of type IHypermediaAppender<T> whilst we define this logic in the base class of T

            var methodInfo = type.GetMethod("Append", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.NonPublic);
            var genericMethod = methodInfo.MakeGenericMethod(type);

            genericMethod.Invoke(this, new object[] {this, resolver});
        }

        protected static void Append<T>(IResource resource, IHypermediaResolver resolver) where T : class, IResource // called using reflection ...
        {
            var typed = resource as T;

            var appender = resolver.ResolveAppender(typed);
            var configured = resolver.ResolveLinks(typed).ToList();

            configured.Insert(0, resolver.ResolveSelf(typed));

            appender.Append(typed, configured);
        }

        internal static bool IsEmbeddedResourceType(Type type)
        {
            return typeof (IResource).IsAssignableFrom(type) ||
                   typeof (IEnumerable<IResource>).IsAssignableFrom(type);
        }

        [Obsolete("Only used by the XmlHalMediaTypeFormatter and the ResourceListConverter")]
        public void RepopulateHyperMedia()
        {
            CreateHypermedia();

            if (!string.IsNullOrEmpty(Href) && Links.Count(l=>l.Rel == "self") == 0)
                Links.Insert(0, new Link { Rel = "self", Href = Href });
        }

        [JsonIgnore]
        public virtual string Rel { get; set; }

        [JsonIgnore]
        public virtual string Href { get; set; }

        [JsonIgnore]
        public string LinkName { get; set; }

        public IList<Link> Links { get; set; }

        protected internal virtual void CreateHypermedia()
        {
        }
    }
}
