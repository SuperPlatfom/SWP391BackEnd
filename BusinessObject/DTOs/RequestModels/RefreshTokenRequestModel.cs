using System.Text.Json.Serialization;

namespace BusinessObject.DTOs.RequestModels;

public class RefreshTokenRequestModel
{
    [JsonPropertyName("refreshToken")] public string RefreshToken { get; set; }
}