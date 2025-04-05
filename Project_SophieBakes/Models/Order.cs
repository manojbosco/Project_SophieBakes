using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project_SophieBakes.Models;

namespace Project_SophieBakes.Models;
public class Order
{
    [Key] // Ensure it's the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-generate ID
    public int OrderId { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    public string Status { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; }

    public string PaymentStatus { get; set; }
    public User User { get; set; } // Navigation property to User


    [Required]
    public string CustomerEmail { get; set; }

    public List<OrderItem> OrderItems { get; set; }
}