using System;
using Newtonsoft.Json;

namespace HalWebApi.JsonConverters
{
    public class ResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resource = (HalResource)value;

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
            return typeof(HalResource).IsAssignableFrom(objectType) &&
                   (!objectType.IsGenericType ||
                    objectType.GetGenericTypeDefinition() != typeof(ResourceList<>));
        }
    }
}