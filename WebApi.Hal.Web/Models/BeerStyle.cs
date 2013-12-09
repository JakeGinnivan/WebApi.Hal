namespace WebApi.Hal.Web.Models
{
    public class BeerStyle
    {
        protected BeerStyle()
        {
        }

        public int Id { get; protected set; }
        public string Name { get; set; }
    }
}