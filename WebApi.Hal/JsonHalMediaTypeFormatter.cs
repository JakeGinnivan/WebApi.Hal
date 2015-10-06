using Newtonsoft.Json;
using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal
{
    public class JsonHalMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private readonly ResourceListConverter resourceListConverter = new ResourceListConverter();
        private readonly ResourceConverter resourceConverter = new ResourceConverter();
        private readonly LinksConverter linksConverter = new LinksConverter();
        private readonly EmbeddedResourceConverter embeddedResourceConverter = new EmbeddedResourceConverter();
        private readonly IHypermediaResolver hypermediaConfiguration;

        public JsonHalMediaTypeFormatter(IHypermediaResolver hypermediaConfiguration)
        {
            if (hypermediaConfiguration == null)
                throw new ArgumentNullException("hypermediaConfiguration");

            resourceConverter = new ResourceConverter(hypermediaConfiguration);
            Initialize();
        }

        public JsonHalMediaTypeFormatter()
        {
            resourceConverter = new ResourceConverter();
            Initialize();
        }

        private void Initialize()
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

        /// <summary>
        /// Force the response content-type header to be application/hal+json even if the request is a MediaType supported
        /// by the JsonMediaTypeFormatter (ie application/json)
        /// </summary>
        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/hal+json");
        }
    }
}