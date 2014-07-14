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
        private IList<EmbeddedResource> Embedded { get; set; }

        [JsonIgnore]
        readonly IDictionary<PropertyInfo, object> embeddedResourceProperties = new Dictionary<PropertyInfo, object>();
        
        [OnSerializing]
        private void OnSerialize(StreamingContext context)
        {
            // Clear the embeddedResourceProperties in order to make this object re-serializable.
            embeddedResourceProperties.Clear();

            RepopulateHyperMedia();

            if (ResourceConverter.IsResourceConverterContext(context))
            {
                // put all embedded resources and lists of resources into Embedded for the _embedded serializer
                Embedded = new List<EmbeddedResource>();
                foreach (var prop in GetType().GetProperties().Where(p => IsEmbeddedResourceType(p.PropertyType)))
                {
                    var val = prop.GetValue(this, null);
                    if (val == null) continue;
                    // remember embedded resource property for restoring after serialization
                    embeddedResourceProperties.Add(prop, val);
                    // add embedded resource to collection for the serializtion
                    var res = val as IResource;
                    var embeddedResource = new EmbeddedResource();
                    if (res != null)
                    {
                        embeddedResource.IsSourceAnArray = false;
                        embeddedResource.Resources.Add(res);
                        Embedded.Add(embeddedResource);
                    }
                    else
                    {
                        var resEnum = val as IEnumerable<IResource>;
                        if (resEnum != null)
                        {
                            var resList = resEnum.ToList();
                            if (resList.Count > 0)
                            {
                                embeddedResource.IsSourceAnArray = true;
                                foreach (var resElem in resList)
                                    embeddedResource.Resources.Add(resElem);
                                Embedded.Add(embeddedResource);
                            }
                        }
                    }
                    // null out the embedded property so it doesn't serialize separately as a property
                    prop.SetValue(this, null, null);
                }
                if (Embedded.Count == 0)
                    Embedded = null;
            }
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

        protected internal abstract void CreateHypermedia();
    }

    internal class EmbeddedResource
    {
        public EmbeddedResource()
        {
            Resources = new List<IResource>();
        }

        public bool IsSourceAnArray { get; set; }
        public IList<IResource> Resources { get; private set; }
    }
}
