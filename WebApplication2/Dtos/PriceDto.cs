using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dtos
{
    public class PriceDto
    {
        [Required]
        public Double Lat1{ get; set; }
        [Required]
        public Double Long1 { get; set; }
        [Required]
        public Double Lat2 { get; set; }

        [Required]
        public Double Long2 { get; set; }
    }
}
