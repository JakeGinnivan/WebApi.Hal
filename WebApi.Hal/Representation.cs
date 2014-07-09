using System;
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
        private ILookup<string, IResource> Embedded { get; set; }

        [JsonIgnore]
        readonly IDictionary<PropertyInfo, object> embeddedResourceProperties = new Dictionary<PropertyInfo, object>();

        [OnSerializing]
        private void OnSerialize(StreamingContext context)
        {
            if (!ResourceConverter.IsResourceConverterContext(context))
                return;

            var ctx = (HalJsonConverterContext) context.Context;

            if (ctx.HypermediaConfiguration != null)
            {
                if (!ctx.IsRoot)
                    return;

                ctx.HypermediaConfiguration.Configure(this);
            }
            else
            {
                RepopulateHyperMedia();
                RepopulateEmbeddedResources();
            }

            if (ctx.IsRoot)
                ctx.IsRoot = false;
        }

        internal void RepopulateEmbeddedResources(Func<IResource, string> getRel = null)
        {
            // put all embedded resources and lists of resources into Embedded for the _embedded serializer
            var resourceList = new List<IResource>();
            foreach (var prop in GetType().GetProperties().Where(p => IsEmbeddedResourceType(p.PropertyType)))
            {
                var val = prop.GetValue(this, null);
                if (val != null)
                {
                    // remember embedded resource property for restoring after serialization
                    embeddedResourceProperties.Add(prop, val);
                    // add embedded resource to collection for the serializtion
                    var res = val as IResource;
                    if (res != null)
                        resourceList.Add(res);
                    else
                        resourceList.AddRange((IEnumerable<IResource>) val);
                    // null out the embedded property so it doesn't serialize separately as a property
                    prop.SetValue(this, null, null);
                }
            }
            
            foreach (var res in resourceList.Where(r => string.IsNullOrEmpty(r.Rel)))
                res.Rel = "unknownRel-" + res.GetType().Name;

            Embedded = resourceList.Any()
                ? resourceList.ToLookup(
                    r => getRel != null
                        ? getRel(r)
                        : r.Rel)
                : null;
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

        internal static bool IsEmbeddedResourceType(Type type)
        {
            return typeof (IResource).IsAssignableFrom(type) ||
                   typeof (IEnumerable<IResource>).IsAssignableFrom(type);
        }

        public void RepopulateHyperMedia()
        {
            CreateHypermedia();
            if (Links.Count(l=>l.Rel == "self") == 0)
                Links.Insert(0, new Link { Rel = "self", Href = Href });
        }

        [JsonIgnore]
        public virtual string Rel { get; set; }

        [JsonIgnore]
        public virtual string Href { get; set; }

        [JsonIgnore]
        public string LinkName { get; set; }

        public List<Link> Links { get; set; }

        protected internal virtual void CreateHypermedia()
        {
        }

        internal IEnumerable<Link> GetHypermediaDeep()
        {
            foreach (var link in Links)
                yield return link;

            if (Embedded == null)
                yield break;

            var embedded = Embedded.SelectMany(x => x).OfType<Representation>().SelectMany(x => x.GetHypermediaDeep());

            foreach (var link in embedded)
                yield return link;
        }

        internal void RepopulateAndConfigureDeep(Action<IResource> appendHypermedia, Func<IResource,string> getRel)
        {
            RepopulateEmbeddedResources(getRel);

            appendHypermedia(this);

            if (Embedded == null)
                return;

            foreach (var embedded in Embedded.SelectMany(x => x).OfType<Representation>())
                embedded.RepopulateAndConfigureDeep(appendHypermedia, getRel);
        }
    }
}