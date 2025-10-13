using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class UpdateAccountStatusRequestModel
    {
        [Required]
        [RegularExpression("^(active|inactive)$", ErrorMessage = "Status must be either 'active' or 'inactive'.")]
        public string Status { get; set; }
    }
}
