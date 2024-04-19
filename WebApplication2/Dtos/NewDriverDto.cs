using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;

namespace WebApplication2.Dtos
{
    public class NewDriverDto
    {
        [Key]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String? Email { get; set; }
        [Required]
        public String? Username { get; set; }
        [Required]
        public int Gender { get; set; }
        [Required]
        public String? CarType { get; set; }
        [Required]
        public String? City { get; set; }
        [Required]
        public Boolean Smoking { get; set; }
        [Required]
        public String? Region { get; set; }

        [Required]
        public String? Password { get; set; }
        [Required]
        public String? ConfirmPassword { get; set; }
        public String Role = "Driver";
    }
}
