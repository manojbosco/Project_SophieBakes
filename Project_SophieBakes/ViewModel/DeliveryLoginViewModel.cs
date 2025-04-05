using System.ComponentModel.DataAnnotations;

namespace Project_SophieBakes.Models
{
    public class DeliveryLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}