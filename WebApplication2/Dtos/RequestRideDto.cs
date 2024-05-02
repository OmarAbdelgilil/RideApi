using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dtos
{
    public class RequestRideDto
    {
        [Required]
        public String? PassangerEmail { get; set; }
        [Required]
        public String? DriverEmail { get; set; }
        [Required]
        public String? From { get; set;}

        [Required]
        public String? To { get; set;}
        [Required]
        public double Lat1 { get; set;}
        [Required]
        public double Lat2 { get; set;}
        [Required]
        public double Long1 { get; set;}
        [Required]
        public double Long2 { get; set;}

    }
}
