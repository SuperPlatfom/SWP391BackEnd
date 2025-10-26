using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class UpdateServiceJobStatusRequest
    {
        public string Status { get; set; } = string.Empty; // IN_PROGRESS / DONE / CANCELED
    }

    public class UpdateServiceJobReportRequest
    {
        public string ReportUrl { get; set; } = string.Empty;
    }
}
