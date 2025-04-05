using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Address { get; set; }
 
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; set; } // Add this property


    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    [Required, Compare("Password"), DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}