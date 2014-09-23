using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApi.Hal.JsonConverters
{
    public class LinksConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var links = new HashSet<Link>((IList<Link>)value, new LinkEqualityComparer());
            var lookup = links.ToLookup(l => l.Rel);
            if (lookup.Count == 0) return;

            writer.WriteStartObject();

            foreach (var rel in lookup)
            {
                writer.WritePropertyName(rel.Key);
                if (rel.Count() > 1)
                    writer.WriteStartArray();
                foreach (var link in rel)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("href");
                    writer.WriteValue(ResolveUri(link.Href));

                    if (link.IsTemplated)
                    {
                        writer.WritePropertyName("templated");
                        writer.WriteValue(true);
                    }
                    WriteIfNotNullOrEmpty(writer, link.Title, "title");
                    WriteIfNotNullOrEmpty(writer, link.Profile, "profile");
                    WriteIfNotNullOrEmpty(writer, link.Type, "type"); 
                   

                    writer.WriteEndObject();
                }
                if (rel.Count() > 1)
                    writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        private static void WriteIfNotNullOrEmpty(JsonWriter writer, string val, string name)
        {
            if (!string.IsNullOrEmpty(val))
            {
                writer.WritePropertyName(name);
                writer.WriteValue(val);
            }
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

        public string ResolveUri(string href)
        {
            if (!string.IsNullOrEmpty(href) && VirtualPathUtility.IsAppRelative(href))
                return HttpContext.Current != null ? VirtualPathUtility.ToAbsolute(href) : href.Replace("~/", "/");
            return href;
        }
    }
}