using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal.JsonConverters
{
    public class EmbeddedResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resourceList = (ILookup<string, IResource>)value;
            if (resourceList.Count == 0) return;

            writer.WriteStartObject();

            foreach (var rel in resourceList)
            {
                writer.WritePropertyName(rel.Key);
                if (rel.Count() > 1)
                    writer.WriteStartArray();
                foreach (var res in rel)
                    serializer.Serialize(writer, res);
                if (rel.Count() > 1)
                    writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ILookup<string, IResource>).IsAssignableFrom(objectType);
        }
    }
}
