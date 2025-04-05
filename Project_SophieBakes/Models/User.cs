using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string Address { get; set; }
    
    [Required]
    public override string PhoneNumber { get; set; }
    public string Name { get; set; }

}