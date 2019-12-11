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
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ResourceConverter(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public ResourceConverter(IHypermediaResolver hypermediaConfiguration, JsonSerializerSettings jsonSerializerSettings) : this(jsonSerializerSettings)
        {
            if (hypermediaConfiguration == null)
            {
                throw new ArgumentNullException(nameof(hypermediaConfiguration));
            }

            HypermediaResolver = hypermediaConfiguration;
        }

        public IHypermediaResolver HypermediaResolver { get; }

        private HalJsonConverterContext GetResourceConverterContext()
        {
            var context = (HypermediaResolver == null)
                ? new HalJsonConverterContext()
                : new HalJsonConverterContext(HypermediaResolver);

            return context;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resource = (IResource)value;
			var linksBackup = resource.Links;

            if (linksBackup.Count == 0)
                resource.Links = null; // avoid serialization

            resource.ConverterContext = GetResourceConverterContext();

            var localJsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                CheckAdditionalContent = _jsonSerializerSettings.CheckAdditionalContent,
                ConstructorHandling = _jsonSerializerSettings.ConstructorHandling,
                Context = serializer.Context,
                ContractResolver = _jsonSerializerSettings.ContractResolver,
                Converters = _jsonSerializerSettings.Converters.Where(converter => converter != this).ToList(),
                Culture = _jsonSerializerSettings.Culture,
                DateFormatHandling = _jsonSerializerSettings.DateFormatHandling,
                DateFormatString = _jsonSerializerSettings.DateFormatString,
                DateParseHandling = _jsonSerializerSettings.DateParseHandling,
                DateTimeZoneHandling = _jsonSerializerSettings.DateTimeZoneHandling,
                DefaultValueHandling = _jsonSerializerSettings.DefaultValueHandling,
                EqualityComparer = _jsonSerializerSettings.EqualityComparer,
                Error = _jsonSerializerSettings.Error,
                FloatFormatHandling = _jsonSerializerSettings.FloatFormatHandling,
                FloatParseHandling = _jsonSerializerSettings.FloatParseHandling,
                Formatting = _jsonSerializerSettings.Formatting,
                MaxDepth = _jsonSerializerSettings.MaxDepth,
                MetadataPropertyHandling = _jsonSerializerSettings.MetadataPropertyHandling,
                MissingMemberHandling = _jsonSerializerSettings.MissingMemberHandling,
                NullValueHandling = _jsonSerializerSettings.NullValueHandling,
                ObjectCreationHandling = _jsonSerializerSettings.ObjectCreationHandling,
                PreserveReferencesHandling = _jsonSerializerSettings.PreserveReferencesHandling,
                ReferenceLoopHandling = _jsonSerializerSettings.ReferenceLoopHandling,
                ReferenceResolverProvider = _jsonSerializerSettings.ReferenceResolverProvider,
                SerializationBinder = _jsonSerializerSettings.SerializationBinder,
                StringEscapeHandling = _jsonSerializerSettings.StringEscapeHandling,
                TraceWriter = _jsonSerializerSettings.TraceWriter,
                TypeNameAssemblyFormatHandling = _jsonSerializerSettings.TypeNameAssemblyFormatHandling,
                TypeNameHandling = _jsonSerializerSettings.TypeNameHandling
            });
            localJsonSerializer.Serialize(writer, resource);

            if (linksBackup.Count == 0)
                resource.Links = linksBackup;
		}

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            // let exceptions leak out of here so ordinary exception handling in the server or client pipeline can take place
            return CreateResource(JObject.Load(reader), objectType);
        }

        private const string HalLinksName = "_links";
        private const string HalEmbeddedName = "_embedded";

        private static IResource CreateResource(JObject jObj, Type resourceType)
        {
            // remove _links and _embedded so those don't try to deserialize, because we know they will fail
            if (jObj.TryGetValue(HalLinksName, out JToken links))
                jObj.Remove(HalLinksName);
            if (jObj.TryGetValue(HalEmbeddedName, out JToken embeddeds))
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
                    if (typeof (IEnumerable<IResource>).IsAssignableFrom(prop.PropertyType))
                    {
                        var lst = prop.GetValue(resource) as IList;
                        if (lst == null)
                        {
                            lst = ConstructResource(prop.PropertyType) as IList ??
                                  Activator.CreateInstance(
                                      typeof (List<>).MakeGenericType(prop.PropertyType.GenericTypeArguments)) as IList;
                            if (lst == null) continue;
                            prop.SetValue(resource, lst);
                        }
                        if (prop.PropertyType.GenericTypeArguments != null &&
                            prop.PropertyType.GenericTypeArguments.Length > 0)
                            CreateEmbedded(embeddeds, prop.PropertyType.GenericTypeArguments[0],
                                newRes => lst.Add(newRes));
                    }
                    else
                    {
                        var prop1 = prop;
                        CreateEmbedded(embeddeds, prop.PropertyType, newRes => prop1.SetValue(resource, newRes));
                    }
                }
            }

            return resource;
        }

        static void CreateLinks(JProperty rel, IResource resource)
        {
            if (rel.Value.Type == JTokenType.Array)
            {
                if (rel.Value is JArray arr)
                    foreach (var link in arr.Select(item => item.ToObject<Link>()))
                    {
                        link.Rel = rel.Name;
                        link.IsMultiLink = true;
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
                var tok = embeddeds[rel];
                if (tok != null)
                {
                    switch (tok.Type)
                    {
                        case JTokenType.Array:
                            {
                                if (tok is JArray embeddedJArr)
                                {
                                    foreach (var embeddedJObj in embeddedJArr.OfType<JObject>())
                                        addCreatedResource(CreateResource(embeddedJObj, resourceType)); // recursion
                                }
                            }
                            break;
                        case JTokenType.Object:
                            {
                                if (tok is JObject embeddedJObj)
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
                    if (ConstructResource(resourceType) is IResource res)
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

        static object ConstructResource(Type resourceType)
        {
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
            if (useThisCtor == null) return null;
            var ctorParams = new object[useThisCtor.GetParameters().Length];
            return useThisCtor.Invoke(ctorParams);
        }

        public override bool CanConvert(Type objectType)
        {
            return IsResource(objectType);
        }

        private static bool IsResource(Type objectType)
        {
            return typeof(Representation).IsAssignableFrom(objectType);
        }
    }
}