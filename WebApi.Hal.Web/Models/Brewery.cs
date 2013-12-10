namespace WebApi.Hal.Web.Models
{
    public class Brewery
    {
        protected Brewery()
        {
        }

        public int Id { get; protected set; }
        public string Name { get; set; }
    }
}