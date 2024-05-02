using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [FileExtensions(Extensions = "jpg,jpeg,png")]
        [DataType(DataType.ImageUrl)]
        public  ICollection<Rides>? Rides { get; set; }


    }
}
