using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Text.Json;

namespace WebApi.Hal.Web
{
    public class FormattersMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly JsonSerializerOptions _jsonSerializerSettings;
        private readonly JsonOptions _mvcJsonOptions;

        public FormattersMvcOptionsSetup(
            ILoggerFactory loggerFactory,
            IOptions<JsonOptions> jsonOptions)
        {
            if (jsonOptions == null)
            {
                throw new ArgumentNullException(nameof(jsonOptions));
            }

            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _jsonSerializerSettings = jsonOptions.Value.JsonSerializerOptions;
            
            HalSerializerOptions.GetOrCreateSingleton(ref _jsonSerializerSettings);


            _mvcJsonOptions = jsonOptions.Value;
        }

        public void Configure(MvcOptions options)
        {
            options.OutputFormatters.Add(new XmlHalMediaTypeOutputFormatter());
            options.OutputFormatters.Add(new JsonHalMediaTypeOutputFormatter(_jsonSerializerSettings, null, options));

            // Register JsonPatchInputFormatter before JsonInputFormatter, otherwise
            // JsonInputFormatter would consume "application/json-patch+json" requests
            // before JsonPatchInputFormatter gets to see them.

            var jsonInputPatchLogger = _loggerFactory.CreateLogger<JsonHalMediaTypeInputFormatter>();
            options.InputFormatters.Add(new JsonHalMediaTypeInputFormatter(
                                            jsonInputPatchLogger,
                                            HalSerializerOptions.Default,
                                            null,
                                            null,
                                            options, 
                                            _mvcJsonOptions));
        }
    }
}