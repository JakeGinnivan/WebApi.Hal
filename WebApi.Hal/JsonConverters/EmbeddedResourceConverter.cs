using System;
using System.Linq;
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
                var count = rel.Count();
                if (count == 0) continue;

                writer.WritePropertyName(rel.Key);
                if (count > 1)
                {
                    writer.WriteStartArray();
                    foreach (var res in rel)
                    {
                        serializer.Serialize(writer, res);
                    }
                    writer.WriteEndArray();
                }
                else 
                {
                    serializer.Serialize(writer, rel.Single());
                }
            }
            writer.WriteEndObject();
        }

        public override bool CanRead
        {
            get { return false; }
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
