using System.ComponentModel.DataAnnotations;

namespace ConestogaCollegeMerchandise.Models
{
    public class Merchandise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}
