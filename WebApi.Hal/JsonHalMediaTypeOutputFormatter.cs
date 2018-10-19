using System;
using System.Buffers;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal
{
    public class JsonHalMediaTypeOutputFormatter : JsonOutputFormatter
    {
        private const string _mediaTypeHeaderValueName = "application/hal+json";

        private readonly LinksConverter _linksConverter = new LinksConverter();

        private readonly ResourceConverter _resourceConverter;
        private readonly EmbeddedResourceConverter _embeddedResourceConverter = new EmbeddedResourceConverter();

        public JsonHalMediaTypeOutputFormatter(
            JsonSerializerSettings serializerSettings, 
            ArrayPool<char> charPool, 
            IHypermediaResolver hypermediaResolver) : 
            base(serializerSettings, charPool)
        {
            if (hypermediaResolver == null)
            {
                throw new ArgumentNullException(nameof(hypermediaResolver));
            }

            _resourceConverter = new ResourceConverter(hypermediaResolver, SerializerSettings);
            Initialize();
        }

        public JsonHalMediaTypeOutputFormatter(
            JsonSerializerSettings serializerSettings, 
            ArrayPool<char> charPool) :
            base(serializerSettings, charPool)
        {
            _resourceConverter = new ResourceConverter(SerializerSettings);
            Initialize();
        }

        private void Initialize()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(_mediaTypeHeaderValueName));
            SerializerSettings.Converters.Add(_linksConverter);
            SerializerSettings.Converters.Add(_resourceConverter);
            SerializerSettings.Converters.Add(_embeddedResourceConverter);
            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
        
        protected override bool CanWriteType(Type type)
        {
            return typeof(Representation).IsAssignableFrom(type);
        }
    }
}
