using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class AccountRequestModel
    {
        public string FullName { get; set; }

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Password cannot be blank")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,16}$",
            ErrorMessage = "Minimum 8 characters, at least one uppercase letter and one number")]
        public string Password { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public bool Gender { get; set; }

        public string Phone { get; set; }

        public int RoleId { get; set; }

    }
}
