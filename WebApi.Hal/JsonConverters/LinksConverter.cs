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

            writer.WriteStartObject();

            var lookup = links.ToLookup(l => l.Rel);

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

                    var remaining = link.GetType()
                        .GetProperties()
                        .Where(x => !x.Name.Equals("Href") && !x.Name.Equals("IsTemplated") && !x.Name.Equals("Rel"));

                    foreach (var info in remaining)
                    {
                        if ((info.PropertyType != typeof (string))) 
                            continue; // no sensible way to serialize ...

                        var text = info.GetValue(link) as string;

                        if (string.IsNullOrEmpty(text)) 
                            continue; // no value set, so don't serialize this ...

                        writer.WritePropertyName(info.Name.ToLowerInvariant());
                        writer.WriteValue(text);
                    }

                    writer.WriteEndObject();
                }

                if (rel.Count() > 1)
                    writer.WriteEndArray();
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

        public string ResolveUri(string href)
        {
            if (VirtualPathUtility.IsAppRelative(href))
                return HttpContext.Current != null ? VirtualPathUtility.ToAbsolute(href) : href.Replace("~/", "/");
            return href;
        }
    }
}