using System.ComponentModel.DataAnnotations;

namespace Project_SophieBakes.Models
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}