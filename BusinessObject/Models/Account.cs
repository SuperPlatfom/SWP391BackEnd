using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("account")]
    public class Account
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Column("gender")]
        public bool Gender { get; set; }

        [Column("email")]
        public string Email { get; set; } 

        [Column("password_hash")]
        public string PasswordHash { get; set; }  

        [Column("phone")]
        public string Phone { get; set; }  

        [Column("image_url")]
        public string? ImageUrl { get; set; } 

        [Column("role_id")]
        public int RoleId { get; set; }     

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("is_logged_in")]
        public bool IsLoggedIn { get; set; }
        [Column("account_status")]
        public string Status { get; set; } 

        [Column("refresh_token")]
        public string? RefreshToken { get; set; }  

        [Column("refresh_token_expiry")]
        public DateTime? RefreshTokenExpiry { get; set; }

        [Column("activation_code")]
        public string? ActivationCode { get; set; }

        [Column("code_expiry")]
        public DateTime? CodeExpiry { get; set; }

        public Role Role { get; set; }
        public CitizenIdentityCard? CitizenIdentityCard { get; set; }
        public ICollection<CoOwnershipGroup> CreatedGroups { get; set; } = new List<CoOwnershipGroup>();
        public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<EContract> CreatedContracts { get; set; } = new List<EContract>();
        public ICollection<EContractSigner> SignedContracts { get; set; } = new List<EContractSigner>();
        public ICollection<EContractMemberShare> OwnershipShares { get; set; } = new List<EContractMemberShare>();
     
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<UsageQuota> UsageQuota { get; set; } = new List<UsageQuota>();
        public ICollection<TripEvent> TripEvent { get; set; } = new List<TripEvent>();


        public ICollection<ServiceRequest> CreatedServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<ServiceRequest> AssignedServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<ServiceJob> ServiceJobs { get; set; } = new List<ServiceJob>();
        public ICollection<MemberInvoice> MemberInvoices { get; set; } = new List<MemberInvoice>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<ServiceRequestConfirmation> ServiceRequestConfirmations { get; set; } = new List<ServiceRequestConfirmation>();

    }
}
