using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal
{
    public class JsonHalMediaTypeFormatter : JsonMediaTypeFormatter
    {
        readonly ResourceListConverter resourceListConverter = new ResourceListConverter();
        readonly ResourceConverter resourceConverter = new ResourceConverter();
        readonly LinksConverter linksConverter = new LinksConverter();
        readonly EmbeddedResourceConverter embeddedResourceConverter = new EmbeddedResourceConverter();

        public JsonHalMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
            SerializerSettings.Converters.Add(linksConverter);
            SerializerSettings.Converters.Add(resourceListConverter);
            SerializerSettings.Converters.Add(resourceConverter);
            SerializerSettings.Converters.Add(embeddedResourceConverter);
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public override bool CanReadType(Type type)
        {
            return typeof(Representation).IsAssignableFrom(type);
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(Representation).IsAssignableFrom(type);
        }
    }
}