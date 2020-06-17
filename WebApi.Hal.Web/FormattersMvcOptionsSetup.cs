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
        private readonly MvcNewtonsoftJsonOptions _mvcJsonOptions;

        public FormattersMvcOptionsSetup(
            ILoggerFactory loggerFactory,
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions,
            ArrayPool<char> charPool,
            ObjectPoolProvider objectPoolProvider)
        {
            if (jsonOptions == null)
            {
                throw new ArgumentNullException(nameof(jsonOptions));
            }

            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _jsonSerializerSettings = jsonOptions.Value.SerializerSettings;
            _charPool = charPool ?? throw new ArgumentNullException(nameof(charPool));
            _objectPoolProvider = objectPoolProvider ?? throw new ArgumentNullException(nameof(objectPoolProvider));
            _mvcJsonOptions = jsonOptions.Value;
        }

        public void Configure(MvcOptions options)
        {
            options.OutputFormatters.Add(new XmlHalMediaTypeOutputFormatter());
            options.OutputFormatters.Add(new JsonHalMediaTypeOutputFormatter(_jsonSerializerSettings, _charPool, options));

            // Register JsonPatchInputFormatter before JsonInputFormatter, otherwise
            // JsonInputFormatter would consume "application/json-patch+json" requests
            // before JsonPatchInputFormatter gets to see them.

            var jsonInputPatchLogger = _loggerFactory.CreateLogger<JsonHalMediaTypeInputFormatter>();
            options.InputFormatters.Add(new JsonHalMediaTypeInputFormatter(
                                            jsonInputPatchLogger,
                                            new JsonSerializerSettings(),
                                            _charPool,
                                            _objectPoolProvider,
                                            options, 
                                            _mvcJsonOptions));
        }
    }
}