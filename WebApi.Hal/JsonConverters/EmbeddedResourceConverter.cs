using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WebApi.Hal.JsonConverters
{
    public class EmbeddedResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resourceList = (IList<EmbeddedResource>)value;
           
            writer.WriteStartObject();

            foreach (var rel in resourceList)
            {
                writer.WritePropertyName(NormalizeRel(rel));
                if (rel.IsSourceAnArray)
                    writer.WriteStartArray();
                foreach (var res in rel.Resources)
                    serializer.Serialize(writer, res);
                if (rel.IsSourceAnArray)
                    writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        private static string NormalizeRel(EmbeddedResource relation) =>
            !string.IsNullOrEmpty(relation.RelationName)
                ? relation.RelationName
                : $"unknownRel-{relation.Resources.FirstOrDefault()?.GetType().Name ?? string.Empty}";

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<EmbeddedResource>).IsAssignableFrom(objectType);
        }
    }
}
