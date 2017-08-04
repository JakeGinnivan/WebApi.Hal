using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace WebApi.Hal.JsonConverters
{
    public class LinksConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var links = new HashSet<Link>((IList<Link>)value, Link.EqualityComparer);
            var lookup = links.ToLookup(l => l.Rel);
            
            if (lookup.Count == 0) 
                return;

            writer.WriteStartObject();

            foreach (var rel in lookup)
            {
                var count = rel.Count();

                writer.WritePropertyName(rel.Key);
                
                if ((count > 1) || (rel.Key == Link.RelForCuries))
                    writer.WriteStartArray();

                foreach (var link in rel)
                    WriteLink(writer, link);

                if ((count > 1) || (rel.Key == Link.RelForCuries))
                    writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        void WriteLink(JsonWriter writer, Link link)
        {
            writer.WriteStartObject();

            foreach (var info in link.GetType().GetProperties())
            {
                switch (info.Name.ToLowerInvariant())
                {
                    case "href":
                        writer.WritePropertyName("href");
                        writer.WriteValue(ResolveUri(link.Href));
                        break;
                    case "rel":
                        // do nothing ...
                        break;
                    case "istemplated":
                        if (link.IsTemplated)
                        {
                            writer.WritePropertyName("templated");
                            writer.WriteValue(true);
                        }
                        break;
                    default:
                        if ((info.PropertyType == typeof (string)))
                        {
                            var text = info.GetValue(link) as string;

                            if (string.IsNullOrEmpty(text))
                                continue; // no value set, so don't write this property ...

                            writer.WritePropertyName(info.Name.ToLowerInvariant());
                            writer.WriteValue(text);
                        }
                        // else: no sensible way to serialize ...
                        break;
                }
            }

            writer.WriteEndObject();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<Link>).IsAssignableFrom(objectType);
        }

        public virtual string ResolveUri(string href)
        {
            return href;
        }
    }
}