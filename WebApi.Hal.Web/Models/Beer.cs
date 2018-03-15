using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("Style")]
        public int Style_Id { get; set; }

        public BeerStyle Style { get; set; }

        [ForeignKey("Brewery")]
        public int Brewery_Id { get; set; }

        public Brewery Brewery { get; set; }
    }
}