namespace WebApi.Hal.Web.Api.Resources
{
    public class BeerResource : Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brewery { get; set; }
        public string Style { get; set; }
    }
}
