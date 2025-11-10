using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Hal.JsonConverters
{
    public class EmbeddedResourceConverter : JsonConverter<IList<EmbeddedResource>>
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<EmbeddedResource>).IsAssignableFrom(objectType);
        }

        public override IList<EmbeddedResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IList<EmbeddedResource> resourceList, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var rel in resourceList)
            {
                writer.WritePropertyName(NormalizeRel(rel));
                if (rel.IsSourceAnArray)
                    writer.WriteStartArray();
                foreach (var res in rel.Resources)
                    writer.WriteStringValue(res.ToString()); // TODO: Implement proper serialization for IResource
                if (rel.IsSourceAnArray)
                    writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        private static string NormalizeRel(EmbeddedResource relation) =>
            !string.IsNullOrEmpty(relation.RelationName)
                ? relation.RelationName
                : $"unknownRel-{relation.Resources.FirstOrDefault()?.GetType().Name ?? string.Empty}";

    }
}
