using System.ComponentModel.DataAnnotations;
namespace WebApplication2.Dtos
{
    public class ChangePasswordDto
    {
        [Required]
        public string? OldPassword { get; set; }
        [Required]
        public string? NewPassword { get; set; }

        [Required]
        public string? Email { get; set; }

    }
}
