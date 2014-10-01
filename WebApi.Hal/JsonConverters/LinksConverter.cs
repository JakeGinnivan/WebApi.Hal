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
            var pureLinks = ((IList<Link>) value).Where(q => q.GetType() != typeof (Curie)).ToList();
            var curies = ((IList<Link>)value).Where(q => q.GetType() == typeof(Curie)).ToList();
            var links = new HashSet<Link>(pureLinks, new LinkEqualityComparer());
            var curiedLinks = new HashSet<Link>(curies, new LinkEqualityComparer());
            var lookup = links.ToLookup(l => l.Rel);
            if (lookup.Count == 0) return;

            writer.WriteStartObject();
            bool hasCuries = curiedLinks.Count > 0;
            if (hasCuries)
            {
                writer.WritePropertyName("curies");
                writer.WriteStartArray();
            
                foreach (var rel in curiedLinks)
                {
                    var curie = rel as Curie;
                    WriteCurie(writer, curie);
                }
            
                writer.WriteEndArray();
            }

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

        void WriteCurie(JsonWriter writer, Curie curie)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            writer.WriteValue(curie.Rel);
            writer.WritePropertyName("href");
            writer.WriteValue(ResolveUri(curie.Href));

            if (curie.IsTemplated)
            {
                writer.WritePropertyName("templated");
                writer.WriteValue(true);
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