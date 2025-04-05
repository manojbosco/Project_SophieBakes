using System.ComponentModel.DataAnnotations;

namespace Project_SophieBakes.Models
{
    public class Product
    {
        public int ProductId { get; set; }
    
        [Required]
        public string ProductName { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; } // ✅ Ensure this exists

        public string ImageUrl { get; set; }
      

        // Navigation property
        public Category Category { get; set; }
    }

}