﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class ChangePasswordRequestModel
    {
        [JsonPropertyName("oldPassword")]
        [Required(ErrorMessage = " Old Password cannot be blank")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,16}$",
        ErrorMessage = "Minimum 8 characters, at least one uppercase letter and one number")]
        public string OldPassword { get; set; } = string.Empty;

        [JsonPropertyName("newPassword")]
        [Required(ErrorMessage = "New Password cannot be blank")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,16}$",
        ErrorMessage = "Minimum 8 characters, at least one uppercase letter and one number")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
