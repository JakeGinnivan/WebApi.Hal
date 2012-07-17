using System;
using Newtonsoft.Json;

namespace WebApi.Hal.JsonConverters
{
    public class ResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resource = (Resource)value;

            resource.Links.Add(new Link
                                   {
                                       Rel = "self",
                                       Href = resource.Href
                                   });

            serializer.Converters.Remove(this);
            serializer.Serialize(writer, resource);
            serializer.Converters.Add(this);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return IsResource(objectType) && !IsResourceList(objectType);
        }

        static bool IsResourceList(Type objectType)
        {
            return typeof(IResourceList).IsAssignableFrom(objectType);
        }

        static bool IsResource(Type objectType)
        {
            return typeof(Resource).IsAssignableFrom(objectType);
        }
    }
}