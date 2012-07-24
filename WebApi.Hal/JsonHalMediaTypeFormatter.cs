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
        readonly ResourceListConverter resourceListConverter = new ResourceListConverter();
        readonly ResourceConverter resourceConverter = new ResourceConverter();
        readonly LinksConverter linksConverter = new LinksConverter();

        public JsonHalMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
            SerializerSettings.Converters.Add(linksConverter);
            SerializerSettings.Converters.Add(resourceListConverter);
            SerializerSettings.Converters.Add(resourceConverter);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            return base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext)
                .ContinueWith(t=>
                {
                    SerializerSettings.Converters.Remove(linksConverter);
                    SerializerSettings.Converters.Remove(resourceListConverter);
                    SerializerSettings.Converters.Remove(resourceConverter);
                });
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