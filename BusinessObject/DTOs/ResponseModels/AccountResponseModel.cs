using BusinessObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class AccountResponseModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string Status { get; set; } = "";
        public string? ImageUrl { get; set; }
        public string RoleName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string? CitizenIdNumber { get; set; }
        public string? Address { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? PlaceOfIssue { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? FrontImageUrl { get; set; }
        public string? BackImageUrl { get; set; }


    }
}
