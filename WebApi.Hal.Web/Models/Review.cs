using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Hal.Web.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Beer")]
        public int Beer_Id { get; set; }

        public Beer Beer { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Content { get; set; }
    }
}