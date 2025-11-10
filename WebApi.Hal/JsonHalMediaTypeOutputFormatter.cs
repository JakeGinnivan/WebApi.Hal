using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Buffers;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Hal.Interfaces;
using WebApi.Hal.JsonConverters;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace WebApi.Hal
{
    public class JsonHalMediaTypeOutputFormatter : SystemTextJsonOutputFormatter
    {
        private const string _mediaTypeHeaderValueName = "application/hal+json";

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
        }

        public JsonHalMediaTypeOutputFormatter(
            JsonSerializerOptions serializerSettings, 
            ArrayPool<char> charPool,
            MvcOptions mvcOptions) :
            base(serializerSettings)
        {
        }

        public void WriteObject(TextWriter stream, IResource value)
        {
            // TODO make this slick. might need to update the unit tests as well.
            
            var ss = JsonSerializer.Serialize(value, SerializerOptions);
            
            stream.Write(ss);

        }

        
        protected override bool CanWriteType(Type type)
        {
            return typeof(Representation).IsAssignableFrom(type);
        }
    }
}
