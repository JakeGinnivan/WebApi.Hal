using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal
{
    public class JsonHalMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public JsonHalMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
            SerializerSettings.Converters.Add(new LinksConverter());
            SerializerSettings.Converters.Add(new ResourceListConverter());
            SerializerSettings.Converters.Add(new ResourceConverter());
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            var resource = value as Resource;
            if (resource == null)
                return base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext);

            return base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext);
        }

        public override bool CanReadType(Type type)
        {
            return typeof(Resource).IsAssignableFrom(type);
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(Resource).IsAssignableFrom(type);
        }
    }
}