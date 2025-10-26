using BusinessObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.RequestModels
{
    public class AddAccountRequestModel
    {
        [JsonPropertyName("fullName")]
        [Required(ErrorMessage = "Full name is mandatory")]
        [MinLength(2, ErrorMessage = "Full name must be at least 2 characters long")]
        public string fullName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Password cannot be blank")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,16}$",
            ErrorMessage = "Minimum 8 characters, at least one uppercase letter and one number")]
        public string password { get; set; } = string.Empty;

        [JsonPropertyName("dateOfBirth")]
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime dateOfBirth { get; set; }

        [JsonPropertyName("phone")]
        [Required(ErrorMessage = "Phone cannot be blank")]
        [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b",
            ErrorMessage = "Please enter a valid (+84) phone number")]
        public string phone { get; set; } = string.Empty;

        [JsonPropertyName("gender")]
        [Required(ErrorMessage = "Gender is required")]
        public bool gender { get; set; }

        [JsonPropertyName("idNumber")]
        [Required(ErrorMessage = "ID number is required")]
        [RegularExpression(@"^([0-9]{9}|[0-9]{12})$", ErrorMessage = "ID number must be exactly 9 or 12 digits")]
        public string idNumber { get; set; } = string.Empty;

        [JsonPropertyName("issueDate")]
        [Required(ErrorMessage = "Issue date is required")]
        [DataType(DataType.Date)]
        public DateTime issueDate { get; set; }

        [JsonPropertyName("expiryDate")]
        [Required(ErrorMessage = "Expiry date is required")]
        [DataType(DataType.Date)]
        public DateTime expiryDate { get; set; }

        [JsonPropertyName("placeOfIssue")]
        [Required(ErrorMessage = "Place of issue is required")]
        [MinLength(2, ErrorMessage = "Place of issue must be at least 2 characters long")]
        public string placeOfIssue { get; set; } = string.Empty;

        [JsonPropertyName("placeOfBirth")]
        [Required(ErrorMessage = "Place of birth is required")]
        [MinLength(2, ErrorMessage = "Place of birth must be at least 2 characters long")]
        public string placeOfBirth { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        [Required(ErrorMessage = "Address is required")]
        [MinLength(5, ErrorMessage = "Address must be at least 5 characters long")]
        public string address { get; set; } = string.Empty;

        [JsonPropertyName("frontImage")]
        public IFormFile? frontImage { get; set; }

        [JsonPropertyName("backImage")]
        public IFormFile? backImage { get; set; }

    }
}
