using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_SophieBakes.Models
{
    public class DeliveryOrders
    {
        [Key]
        public int Id { get; set; }  // Update this to match the primary key in your database

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string OrderDetails { get; set; }

        [Required]
        public string Status { get; set; } // Pending, Out for Delivery, Delivered

        public string? DeliveryBoyId { get; set; } // Link to DeliveryBoy user

        [ForeignKey("DeliveryBoyId")]
        public User? DeliveryBoy { get; set; } // Navigation Property

        public DateTime OrderDate { get; set; } = DateTime.Now;
        // Link to User
        public string UserId { get; set; }  // Foreign Key
        public User User { get; set; }  // Navigation Property
    }
}