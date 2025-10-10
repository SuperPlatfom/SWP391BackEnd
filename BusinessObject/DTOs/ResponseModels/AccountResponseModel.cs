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
        public string Address { get; set; } = "";
        public int TotalPoint { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; } = "";
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public string Status { get; set; } = "";
        public bool IsSubscription {  get; set; }
        public string AchievementName { get; set; }
    }

    public class OfficerResponseModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public int TotalPoint { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; } = "";
        public string RoleName { get; set; } = "";
        public string Status { get; set; } = "";
        public string DistrictName { get; set; }
    }
}
