using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal.JsonConverters
{
    public class ResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resource = (IResource)value;

            serializer.Converters.Remove(this);
            serializer.Serialize(writer, resource);
            serializer.Converters.Add(this);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            // let exceptions leak out of here so ordinary exception handling in the server or client pipeline can take place
            return CreateResource(JObject.Load(reader), objectType);
        }

        const string HalLinksName = "_links";
        const string HalEmbeddedName = "_embedded";

        static IResource CreateResource(JObject jObj, Type resourceType)
        {
            // remove _links and _embedded so those don't try to deserialize, because we know they will fail
            JToken links;
            if (jObj.TryGetValue(HalLinksName, out links))
                jObj.Remove(HalLinksName);
            JToken embeddeds;
            if (jObj.TryGetValue(HalEmbeddedName, out embeddeds))
                jObj.Remove(HalEmbeddedName);

            // create value properties in base object
            var resource = jObj.ToObject(resourceType) as IResource;
            if (resource == null) return null;

            // links are named properties, where the name is Link.Rel and the value is the rest of Link
            if (links != null)
            {
                foreach (var rel in links.OfType<JProperty>())
                    CreateLinks(rel, resource);
                var self = resource.Links.SingleOrDefault(l => l.Rel == "self");
                if (self != null)
                    resource.Href = self.Href;
            }

            // embedded are named properties, where the name is the Rel, which needs to map to a Resource Type, and the value is the Resource
            // recursive
            if (embeddeds != null)
            {
                foreach (var prop in resourceType.GetProperties().Where(p => Representation.IsEmbeddedResourceType(p.PropertyType)))
                {
                    // expects embedded collection of resources is implemented as an IList on the Representation-derived class
                    if (typeof (IList).IsAssignableFrom(prop.PropertyType))
                    {
                        var lst = prop.GetValue(resource) as IList;
                        if (lst != null && prop.PropertyType.GenericTypeArguments != null &&
                            prop.PropertyType.GenericTypeArguments.Length > 0)
                            CreateEmbedded(embeddeds, prop.PropertyType.GenericTypeArguments[0],
                                newRes => lst.Add(newRes));
                    }
                    else
                        CreateEmbedded(embeddeds, prop.PropertyType, newRes => prop.SetValue(resource, newRes));
                }
            }

            return resource;
        }

        static void CreateLinks(JProperty rel, IResource resource)
        {
            if (rel.Value.Type == JTokenType.Array)
            {
                var arr = rel.Value as JArray;
                if (arr != null)
                    foreach (var link in arr.Select(item => item.ToObject<Link>()))
                    {
                        link.Rel = rel.Name;
                        resource.Links.Add(link);
                    }
            }
            else
            {
                var link = rel.Value.ToObject<Link>();
                link.Rel = rel.Name;
                resource.Links.Add(link);
            }
        }

        static void CreateEmbedded(JToken embeddeds, Type resourceType, Action<IResource> addCreatedResource)
        {
            var rel = GetResourceTypeRel(resourceType);
            if (!string.IsNullOrEmpty(rel))
            {
                var tok = embeddeds.SelectToken(rel, false);
                if (tok != null)
                {
                    switch (tok.Type)
                    {
                        case JTokenType.Array:
                        {
                            var embeddedJArr = tok as JArray;
                            if (embeddedJArr != null)
                            {
                                foreach (var embeddedJObj in embeddedJArr.OfType<JObject>())
                                    addCreatedResource(CreateResource(embeddedJObj, resourceType)); // recursion
                            }
                        }
                            break;
                        case JTokenType.Object:
                        {
                            var embeddedJObj = tok as JObject;
                            if (embeddedJObj != null)
                                addCreatedResource(CreateResource(embeddedJObj, resourceType)); // recursion
                        }
                            break;
                    }
                }
            }
        }

        // this depends on IResource.Rel being set upon construction
        static readonly IDictionary<string, string> ResourceTypeToRel = new Dictionary<string, string>();
        static readonly object ResourceTypeToRelLock = new object();
        static string GetResourceTypeRel(Type resourceType)
        {
            if (ResourceTypeToRel.ContainsKey(resourceType.FullName))
                return ResourceTypeToRel[resourceType.FullName];
            try
            {
                lock (ResourceTypeToRelLock)
                {
                    if (ResourceTypeToRel.ContainsKey(resourceType.FullName))
                        return ResourceTypeToRel[resourceType.FullName];
                    // favor c-tor with zero params, but if it doesn't exist, use c-tor with fewest params and pass all null values
                    var ctors = resourceType.GetConstructors();
                    ConstructorInfo useThisCtor = null;
                    foreach (var ctor in ctors)
                    {
                        if (ctor.GetParameters().Length == 0)
                        {
                            useThisCtor = ctor;
                            break;
                        }
                        if (useThisCtor == null || useThisCtor.GetParameters().Length > ctor.GetParameters().Length)
                            useThisCtor = ctor;
                    }
                    if (useThisCtor == null) return string.Empty;
                    var ctorParams = new object[useThisCtor.GetParameters().Length];
                    var res = useThisCtor.Invoke(ctorParams) as IResource;
                    if (res != null)
                    {
                        var rel = res.Rel;
                        ResourceTypeToRel.Add(resourceType.FullName, rel);
                        return rel;
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return IsResource(objectType) && !IsResourceList(objectType);
        }

        static bool IsResourceList(Type objectType)
        {
            return typeof(IRepresentationList).IsAssignableFrom(objectType);
        }

        static bool IsResource(Type objectType)
        {
            return typeof(Representation).IsAssignableFrom(objectType);
        }
    }
}