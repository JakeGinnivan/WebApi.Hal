using System;
using System.Buffers;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using WebApi.Hal.JsonConverters;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace WebApi.Hal
{
    public class JsonHalMediaTypeOutputFormatter : SystemTextJsonOutputFormatter
    {
        private const string _mediaTypeHeaderValueName = "application/hal+json";

        private readonly LinksConverter _linksConverter = new LinksConverter();

        private readonly ResourceConverter _resourceConverter;
        private readonly EmbeddedResourceConverter _embeddedResourceConverter = new EmbeddedResourceConverter();

        public JsonHalMediaTypeOutputFormatter(
            JsonSerializerOptions serializerSettings, 
            ArrayPool<char> charPool,
            MvcOptions mvcOptions,
            IHypermediaResolver hypermediaResolver) : 
            base(serializerSettings)
        {
            if (hypermediaResolver == null)
            {
                throw new ArgumentNullException(nameof(hypermediaResolver));
            }

            _resourceConverter = new ResourceConverter(hypermediaResolver, SerializerOptions);
            Initialize();
        }

        public JsonHalMediaTypeOutputFormatter(
            JsonSerializerOptions serializerSettings, 
            ArrayPool<char> charPool,
            MvcOptions mvcOptions) :
            base(serializerSettings)
        {
            _resourceConverter = new ResourceConverter(SerializerOptions);
            Initialize();
        }

        public void WriteObject(TextWriter stream, object value)
        {
            CreateJsonSerializer().Serialize(stream, value);
        }

        private void Initialize()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(_mediaTypeHeaderValueName));
            SerializerOptions.Converters.Add(_linksConverter);
            SerializerOptions.Converters.Add(_resourceConverter);
            SerializerOptions.Converters.Add(_embeddedResourceConverter);
            //SerializerOptions. NullValueHandling = NullValueHandling.Ignore;
        }
        
        protected override bool CanWriteType(Type type)
        {
            return typeof(Representation).IsAssignableFrom(type);
        }
    }
}
