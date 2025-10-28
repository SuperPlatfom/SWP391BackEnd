using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class UpdateInspectionScheduleRequest
    {
        public DateTime InspectionScheduledAt { get; set; }
        public string? InspectionNotes { get; set; }
    }
}
