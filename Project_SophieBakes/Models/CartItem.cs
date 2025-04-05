using System.ComponentModel.DataAnnotations;

namespace Project_SophieBakes.Models
{
    public class CartItem
    {
        [Key] // ✅ Marks this as the primary key
        public int Id { get; set; }
        public string UserId { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; } 

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;

        public Product Product { get; set; } // ✅ Relationship with Product model
    }
}