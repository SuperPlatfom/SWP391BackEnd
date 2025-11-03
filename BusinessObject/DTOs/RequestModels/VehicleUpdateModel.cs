using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class VehicleUpdateModel
    {
        [Required]
        public Guid VehicleId { get; set; }
       
        [Required]
        public string Make { get; set; } = string.Empty;

      
        [Required]
        public string Model { get; set; } = string.Empty;

        [Required, Range(1900, 2100, ErrorMessage = "Invalid model year")]
        public int modelYear { get; set; }

   
        [Required]
        public string color { get; set; } = string.Empty;

  
        [Required, Range(0.1, 500, ErrorMessage = "Battery capacity must be positive")]
        public decimal batteryCapacityKwh { get; set; }

    
        [Required, Range(1, int.MaxValue, ErrorMessage = "Range must be positive")]
        public int rangeKm { get; set; }

        [Required]
        public string plateNumber { get; set; }

      
        [Required]
        public IFormFile? vehicleImage { get; set; }

     
        [Required]
        public IFormFile? registrationPaperUrl { get; set; }
    }
}
