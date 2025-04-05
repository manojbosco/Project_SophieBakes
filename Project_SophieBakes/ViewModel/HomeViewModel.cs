using System.Collections.Generic;

namespace Project_SophieBakes.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public int CartItemCount { get; set; } 
    }
}