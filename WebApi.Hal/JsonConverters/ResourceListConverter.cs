using System;
using System.Collections;
using Newtonsoft.Json;

namespace WebApi.Hal.JsonConverters
{
    public class ResourceListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = (HalResource)value;

            list.Links.Add(new Link
            {
                Rel = "self",
                Href = list.Href
            });

            writer.WriteStartObject();
            writer.WritePropertyName("_links");
            serializer.Serialize(writer, list.Links);

            writer.WritePropertyName("_embedded");
            writer.WriteStartObject();
            writer.WritePropertyName(list.Rel);
            writer.WriteStartArray();
            foreach (HalResource halResource in (IEnumerable)value)
            {
                serializer.Serialize(writer, halResource);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(HalResource).IsAssignableFrom(objectType) &&
                   objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(ResourceList<>);
        }
    }
}