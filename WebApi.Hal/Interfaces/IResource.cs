using System.Collections.Generic;
using System.Text.Json.Serialization;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal.Interfaces
{
    public interface IResource
    {
        [JsonIgnore]
        string Rel { get; set; }

        [JsonIgnore]
        string Href { get; set; }

        [JsonIgnore]
        string LinkName { get; set; }

        [JsonPropertyName("_links")]
        IList<Link> Links { get; set; }

        [JsonIgnore]
        HalJsonConverterContext ConverterContext { get; set; }
    }
}