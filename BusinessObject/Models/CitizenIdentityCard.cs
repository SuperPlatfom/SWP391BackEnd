using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("citizen_identity_card")]
    public class CitizenIdentityCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("issue_date")]
        public DateTime IssueDate { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("place_of_issue")]
        public string PlaceOfIssue { get; set; }

        [Column("place_of_birth")]
        public string PlaceOfBirth { get; set; }

        [Column("id_number")]
        public string IdNumber { get; set; }

        [Column("front_image_url")]
        public string FrontImageUrl { get; set; }

        [Column("back_image_url")]
        public string BackImageUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public Account Account { get; set; }
    }
}
