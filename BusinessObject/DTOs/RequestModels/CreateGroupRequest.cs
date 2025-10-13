using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{

    public class CreateGroupRequest
    {
        public string Name { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public string? GovernancePolicy { get; set; }

        // Tùy chọn: nếu muốn gán sẵn 1 xe cho nhóm mới
        public Guid? VehicleId { get; set; }
    }
}