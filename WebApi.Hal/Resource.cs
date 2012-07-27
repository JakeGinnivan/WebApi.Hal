using System.Collections.Generic;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public abstract class Resource : IResource
    {
        protected Resource()
        {
            Links = new List<Link>();
        }

        [JsonIgnore]
        public string Rel { get; set; }

        [JsonIgnore]
        public string Href { get; set; }

        [JsonIgnore]
        public string LinkName { get; set; }

        [JsonProperty("_links")]
        public List<Link> Links { get; set; }
    }
}