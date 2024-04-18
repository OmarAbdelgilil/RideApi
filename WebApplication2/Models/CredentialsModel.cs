using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Credentials
    {
        [Key]
        public string? Email { get; set; }
        public String? Password { get; set; }
        
        public String? Role { get; set; }

        public int Registered { get; set; } = 1;
    }
}
