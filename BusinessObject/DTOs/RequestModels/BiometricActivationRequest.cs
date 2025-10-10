using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class BiometricActivationRequest
    {
        [Required(ErrorMessage = "Device id cannot be blank")]
        public string DeviceId { get; set; } = null!;
        public bool IsBiometricEnabled { get; set; }
    }
}
