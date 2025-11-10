using System;
using System.Buffers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal
{
    public class JsonHalMediaTypeInputFormatter : SystemTextJsonInputFormatter
    {
        private readonly LinksConverter _linksConverter = new LinksConverter();
        private readonly ResourceConverter _resourceConverter;
        private readonly EmbeddedResourceConverter _embeddedResourceConverter = new EmbeddedResourceConverter();

        public JsonHalMediaTypeInputFormatter(ILogger<JsonHalMediaTypeInputFormatter> logger, JsonSerializerOptions serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider, IHypermediaResolver hypermediaResolver, MvcOptions mvcOptions, JsonOptions mvcJsonOptions) 
            : base(mvcJsonOptions, logger) //, charPool, objectPoolProvider, mvcOptions, mvcJsonOptions)
        {
            if (hypermediaResolver == null)
            {
                throw new ArgumentNullException(nameof(hypermediaResolver));
            }

            _resourceConverter = new ResourceConverter(hypermediaResolver);
            Initialize();
        }

        public JsonHalMediaTypeInputFormatter(ILogger<JsonHalMediaTypeInputFormatter> logger, JsonSerializerOptions serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider, MvcOptions mvcOptions, JsonOptions mvcJsonOptions)
            : base(mvcJsonOptions, logger) //: base(logger, serializerSettings, charPool, objectPoolProvider, mvcOptions, mvcJsonOptions)
        {
            _resourceConverter = new ResourceConverter(null);
            Initialize();
        }

        private void Initialize()
        {
            SerializerOptions.Converters.Add(_linksConverter);
            SerializerOptions.Converters.Add(_resourceConverter);
            SerializerOptions.Converters.Add(_embeddedResourceConverter);
            //SerializerOptions.NullValueHandling = NullValueHandling.Include;
        }
    }
}