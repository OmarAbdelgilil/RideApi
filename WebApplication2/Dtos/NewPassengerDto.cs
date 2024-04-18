using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dtos
{
    public class NewPassengerDto
    {
        [Key]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String? Email { get; set; }

        [Required]
        public String? UserName { get; set; }

        [Required]
        public int Gender { get; set; }

        [Required]
        public String? Password { get; set; }
        [Required]
        public String? ConfirmPassword { get; set; }
        public String Role = "Passenger";


    }
}
