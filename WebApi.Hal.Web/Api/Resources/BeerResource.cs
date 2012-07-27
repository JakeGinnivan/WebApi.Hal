namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerResource : Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int BreweryId { get; set; }
        public string BreweryName { get; set; }

        public int StyleId { get; set; }
        public string StyleName { get; set; }
    }
}
