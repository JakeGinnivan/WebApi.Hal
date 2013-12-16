namespace WebApi.Hal.Web.Data
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