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

   
        
        public string? color { get; set; } = string.Empty;

  
      
        public decimal? batteryCapacityKwh { get; set; }

    
     
        public int? rangeKm { get; set; }

        [Required]
        public string plateNumber { get; set; }

      
        [Required]
        public List <IFormFile?> vehicleImage { get; set; }

     
        [Required]
        public IFormFile? registrationPaperUrl { get; set; }
    }
}
