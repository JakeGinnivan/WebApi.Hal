using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public abstract class Representation : IResource
    {
        protected Representation()
        {
            Links = new HypermediaList();
        }

        public void RepopulateHyperMedia()
        {
            Links.Clear();
            CreateHypermedia();
            if (Links.Count(l=>l.Rel == "self") == 0)
                Links.Insert(0, new Link { Rel = "self", Href = Href });
        }

        [JsonIgnore]
        public string Rel { get; set; }

        [JsonIgnore]
        public string Href { get; set; }

        [JsonIgnore]
        public string LinkName { get; set; }

        public IList<Link> Links { get; set; }

        protected internal abstract void CreateHypermedia();
    }
}