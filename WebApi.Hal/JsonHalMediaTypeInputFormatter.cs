using System;
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal
{
    public class JsonHalMediaTypeInputFormatter : NewtonsoftJsonInputFormatter
    {
        private readonly LinksConverter _linksConverter = new LinksConverter();
        private readonly ResourceConverter _resourceConverter;
        private readonly EmbeddedResourceConverter _embeddedResourceConverter = new EmbeddedResourceConverter();

        public JsonHalMediaTypeInputFormatter(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider, IHypermediaResolver hypermediaResolver, MvcOptions mvcOptions, MvcNewtonsoftJsonOptions mvcJsonOptions) 
            : base(logger, serializerSettings, charPool, objectPoolProvider, mvcOptions, mvcJsonOptions)
        {
            if (hypermediaResolver == null)
            {
                throw new ArgumentNullException(nameof(hypermediaResolver));
            }

            _resourceConverter = new ResourceConverter(hypermediaResolver, SerializerSettings);
            Initialize();
        }

        public JsonHalMediaTypeInputFormatter(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider, MvcOptions mvcOptions, MvcNewtonsoftJsonOptions mvcJsonOptions) 
            : base(logger, serializerSettings, charPool, objectPoolProvider, mvcOptions, mvcJsonOptions)
        {
            _resourceConverter = new ResourceConverter(SerializerSettings);
            Initialize();
        }

        private void Initialize()
        {
            SerializerSettings.Converters.Add(_linksConverter);
            SerializerSettings.Converters.Add(_resourceConverter);
            SerializerSettings.Converters.Add(_embeddedResourceConverter);
            SerializerSettings.NullValueHandling = NullValueHandling.Include;
        }
    }
}