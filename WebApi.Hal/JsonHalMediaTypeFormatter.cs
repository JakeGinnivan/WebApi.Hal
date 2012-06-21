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
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            var resource = value as HalResource;
            if (resource == null)
                return base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext);

            SerializerSettings.Converters.Add(new LinksConverter());
            SerializerSettings.Converters.Add(new ResourceListConverter());
            SerializerSettings.Converters.Add(new ResourceConverter());
            return base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext);
        }

        public override bool CanReadType(Type type)
        {
            return typeof(HalResource).IsAssignableFrom(type);
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(HalResource).IsAssignableFrom(type);
        }
    }


}