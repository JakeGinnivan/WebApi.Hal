using Newtonsoft.Json;

namespace WebApi.Hal.Tests.Proposed.Representations
{
    public class CategoryRepresentation : Representation
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Title { get; set; }
    }
}