using Project_SophieBakes.Models;

namespace Project_SophieBakes.ViewModel
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; } 
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        // ✅ Ensure it's initialized to prevent null reference issues
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}