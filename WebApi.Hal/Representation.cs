using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;

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
            if (string.IsNullOrEmpty(Href) || string.IsNullOrEmpty(Rel) || Links.Count == 0)
                RepopulateHyperMedia();

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
            Embedded = resourceList.Count > 0 ? resourceList.ToLookup(r => r.Rel) : null;
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            // restore embedded resource properties
            foreach (var prop in embeddedResourceProperties.Keys)
                prop.SetValue(this, embeddedResourceProperties[prop], null);
        }

        internal static bool IsEmbeddedResourceType(Type type)
        {
            return typeof (IResource).IsAssignableFrom(type) ||
                   typeof (IEnumerable<IResource>).IsAssignableFrom(type);
        }

        public void RepopulateHyperMedia()
        {
            Links.Clear();
            CreateHypermedia();
            if (Links.Count(l=>l.Rel == "self") == 0)
                Links.Insert(0, new Link { Rel = "self", Href = Href });
        }

        [JsonIgnore]
        public string Rel { get; set; }

        [JsonIgnore]
        public string Href { get; set; }

        [JsonIgnore]
        public string LinkName { get; set; }

        public IList<Link> Links { get; set; }

        protected internal abstract void CreateHypermedia();
    }
}