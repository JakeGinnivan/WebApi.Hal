using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HalWebApi.JsonConverters
{
    public class LinksConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var links = (List<Link>)value;
            writer.WriteStartObject();

            foreach (var link in links)
            {
                writer.WritePropertyName(link.Rel);
                writer.WriteStartObject();
                writer.WritePropertyName("href");
                writer.WriteValue(link.Href);

                if (link.IsTemplated)
                {
                    writer.WritePropertyName("isTemplated");
                    writer.WriteValue(true);
                }

                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(List<Link>) == objectType;
        }
    }
}