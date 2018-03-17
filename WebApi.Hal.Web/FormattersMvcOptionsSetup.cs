using System;
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace WebApi.Hal.Web
{
    public class FormattersMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ArrayPool<char> _charPool;
        private readonly ObjectPoolProvider _objectPoolProvider;

        public FormattersMvcOptionsSetup(
            ILoggerFactory loggerFactory,
            IOptions<MvcJsonOptions> jsonOptions,
            ArrayPool<char> charPool,
            ObjectPoolProvider objectPoolProvider)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (jsonOptions == null)
            {
                throw new ArgumentNullException(nameof(jsonOptions));
            }

            if (charPool == null)
            {
                throw new ArgumentNullException(nameof(charPool));
            }

            if (objectPoolProvider == null)
            {
                throw new ArgumentNullException(nameof(objectPoolProvider));
            }

            _loggerFactory = loggerFactory;
            _jsonSerializerSettings = jsonOptions.Value.SerializerSettings;
            _charPool = charPool;
            _objectPoolProvider = objectPoolProvider;
        }

        public void Configure(MvcOptions options)
        {
            options.OutputFormatters.Add(new XmlHalMediaTypeOutputFormatter());
            options.OutputFormatters.Add(new JsonHalMediaTypeOutputFormatter(_jsonSerializerSettings, _charPool));

            // Register JsonPatchInputFormatter before JsonInputFormatter, otherwise
            // JsonInputFormatter would consume "application/json-patch+json" requests
            // before JsonPatchInputFormatter gets to see them.

            //TODO Including this formatter causes all returns to be empty
            var jsonInputPatchLogger = _loggerFactory.CreateLogger<JsonHalMediaTypeInputFormatter>();
            options.InputFormatters.Add(new JsonHalMediaTypeInputFormatter(
                                            jsonInputPatchLogger,
                                            new JsonSerializerSettings(),
                                            _charPool,
                                            _objectPoolProvider));
        }
    }
}