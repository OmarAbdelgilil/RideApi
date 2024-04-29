using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Credentials
    {
        [Key]
        [Required]
        public string? Email { get; set; }
        [Required]
        public byte[]? PasswordHash { get; set; }
        [Required]
        public byte[]? PasswordSalt { get; set; }
        [Required]
        public String? Role { get; set; }

        public int Registered { get; set; } = 1;
    }
}
