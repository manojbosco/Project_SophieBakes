namespace SophieBakes.Models
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public List<PaymentItemViewModel> CartItems { get; set; } = new List<PaymentItemViewModel>();
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentStatus { get; set; } = "Pending";
        public string ReturnUrl { get; set; } // Add this to handle redirects
    }

    public class PaymentItemViewModel
    {
        public int ProductId { get; set; } // Add this
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity; // Add this
    }
}