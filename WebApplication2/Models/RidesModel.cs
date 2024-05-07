using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Rides
    {
		[Key]
        public String?  Id {  get; set; }

		[Required]
	 public String? Date { get; set; }
        [Required]
        public Double Price { get; set; }
        [Required]
        public String? From { get; set; }
        [Required]
        public String? To { get; set; }

	public int Rate {  get; set; }

	public String? Feedback { get; set; }
        [Required]
        public String? Status { get; set; }
        [Required]
        [ForeignKey("Passanger")]
	public String? PassangerEmail { get; set; }
	public Passanger? Passanger { get; set; }
        [Required]
        [ForeignKey("Driver")]
	public String? DriverEmail { get; set; }
	public Driver? Driver { get; set; }
    [Required]
    public Double Lat1 { get; set; }
    [Required]
    public Double Long1 { get; set; }
    [Required]
    public Double Lat2 { get; set; }

    [Required]
    public Double Long2 { get; set; }
    }
}
