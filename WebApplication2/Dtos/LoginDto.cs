using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
