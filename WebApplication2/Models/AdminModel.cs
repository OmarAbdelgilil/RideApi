using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Admin
    {
        [Required]
        public String? Email { get; set; }
        public String Role = "Admin";
    }
}
