using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_SophieBakes.Models
{
    public class Category
    {
        public int CategoryId { get; set; } // Primary Key

        [Required]
        public string? Name { get; set; } // Category Name
        
        public string? Descriptions { get; set; } 

        // Navigation property for related products
        public virtual List<Product> Products { get; set; } = new List<Product>();
    }
}