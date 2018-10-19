using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Hal.Web.Models
{
    [Table("Beers")]
    public class Beer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [ForeignKey("Style")]
        public int Style_Id { get; set; }

        public BeerStyle Style { get; set; }

        [ForeignKey("Brewery")]
        public int Brewery_Id { get; set; }

        public Brewery Brewery { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? Abv { get; set; }
    }
}