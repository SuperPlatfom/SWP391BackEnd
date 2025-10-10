using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.ResponseModels
{
    public class FptIdCardResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("data")]
        public List<FptIdCardData>? Data { get; set; }
    }

    public class FptIdCardData
    {
        [JsonPropertyName("name")]
        public string? FullName { get; set; }

        [JsonPropertyName("sex")]
        public string? Gender { get; set; }

        [JsonPropertyName("dob")]
        public string? DateOfBirth { get; set; }

        [JsonPropertyName("id")]
        public string? IdNumber { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("home")]
        public string? Home { get; set; }

        [JsonPropertyName("doe")]
        public string? ExpiryDate { get; set; }

        [JsonPropertyName("type")]
        public string? CardSideType { get; set; }

        [JsonPropertyName("issue_date")]
        public string? IssueDate { get; set; }

        [JsonPropertyName("issue_loc")]
        public string? PlaceOfIssue { get; set; }

        [JsonPropertyName("pob")]
        public string? PlaceOfBirth { get; set; }
    }


    public class ParsedIdCardResult
    {
        public string FullName { get; set; }
        public bool? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string IdNumber { get; set; }
        public string Address { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CardSideType { get; set; }
        public DateTime? IssueDate { get; set; }
        public string PlaceOfIssue { get; set; }
        public string PlaceOfBirth { get; set;}
    }

}
