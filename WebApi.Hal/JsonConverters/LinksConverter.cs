using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Hal.JsonConverters
{
    internal sealed class LinksConverter : JsonConverter<IList<Link>>
    {
        public override void Write(Utf8JsonWriter writer, IList<Link> value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }
            var links = new HashSet<Link>(value, Link.EqualityComparer);
            var lookup = links.ToLookup(l => l.Rel);

            if (lookup.Count == 0)
                return;

            writer.WriteStartObject();

            foreach (var rel in lookup)
            {
                var count = rel.Count();

                writer.WritePropertyName(rel.Key);

                bool serializeAsArray = (count > 1) || (rel.Any(l => l.IsMultiLink)) || (rel.Key == Link.RelForCuries);

                if (serializeAsArray)
                    writer.WriteStartArray();

                foreach (var link in rel)
                    WriteLink(writer, link);

                if ((count > 1) || (rel.Any(l => l.IsMultiLink)) || (rel.Key == Link.RelForCuries))
                    writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        public override IList<Link> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is JsonTokenType.Null)
            {
                return null;
            }

            // TODO check this works!
            var list = JsonSerializer.Deserialize<IList<Link>>(ref reader, options);
            return list;
        }

        void WriteLink(Utf8JsonWriter writer, Link link)
        {
            writer.WriteStartObject();

            foreach (var info in link.GetType().GetProperties())
            {
                switch (info.Name.ToLowerInvariant())
                {
                    case "href":
                        writer.WritePropertyName("href");
                        writer.WriteStringValue(ResolveUri(link.Href));
                        break;
                    case "rel":
                        // do nothing ...
                        break;
                    case "istemplated":
                        if (link.IsTemplated)
                        {
                            writer.WritePropertyName("templated");
                            writer.WriteBooleanValue(true);
                        }
                        break;
                    default:
                        if ((info.PropertyType == typeof (string)))
                        {
                            var text = info.GetValue(link) as string;

                            if (string.IsNullOrEmpty(text))
                                continue; // no value set, so don't write this property ...

                            writer.WritePropertyName(info.Name.ToLowerInvariant());
                            writer.WriteStringValue(text);
                        }
                        // else: no sensible way to serialize ...
                        break;
                }
            }

            writer.WriteEndObject();
        }

        private string ResolveUri(string href)
        {
            if (!string.IsNullOrEmpty(href) && href.StartsWith("~"))
                return href.Replace("~/", "/");
            return href;
        }
    }
}