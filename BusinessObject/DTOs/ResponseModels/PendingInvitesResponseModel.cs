using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class PendingInvitesResponseModel
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string RoleInGroup { get; set; } = string.Empty;
        public string InviteStatus { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
    }
}
