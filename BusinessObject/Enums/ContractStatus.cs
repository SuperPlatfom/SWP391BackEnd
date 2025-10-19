using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public static class ContractStatus
    {
        public const string Draft = "DRAFT";
        public const string PendingReview = "PENDING_REVIEW";
        public const string Approved = "APPROVED";
        public const string Rejected = "REJECTED";
        public const string Active = "ACTIVE";
        public const string Expired = "EXPIRED";
    }
}
