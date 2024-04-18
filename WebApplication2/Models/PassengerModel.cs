using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Passanger
    {
        [Key]
        [ForeignKey("Credentials")]
        [Required]
        
        public String? Email { get; set; }

        [Required]
        public String? UserName { get; set; }

        [Required]
        public int Gender { get; set; }

        public ICollection<Rides>? Rides { get; set; }

    }
}
