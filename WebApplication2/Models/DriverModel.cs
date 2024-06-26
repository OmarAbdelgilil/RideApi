﻿using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication2.Models
{
    public class Driver
    {
		[Key]
        [ForeignKey("Credentials")]
        [Required]
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

        
        public Boolean Availability { get; set; } = false;
        
        public Double Rating { get; set; } = 0;
        
        public Boolean Blocked { get; set; } = false;
        [FileExtensions(Extensions = "jpg,jpeg,png")]
        [DataType(DataType.ImageUrl)]
        public string? ImagePath { get; set; }
        public  ICollection<Rides>? Rides { get; set; }

        

    }
}
