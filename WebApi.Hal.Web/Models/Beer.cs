using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Models
{
    public class Beer
    {
        protected Beer()
        {
        }

        public Beer(string name)
        {
            Name = name;
        }

        public int Id { get; protected set; }
        public string Name { get; set; }
        public BeerStyle Style { get; set; }
        public Brewery Brewery { get; set; }
    }
}