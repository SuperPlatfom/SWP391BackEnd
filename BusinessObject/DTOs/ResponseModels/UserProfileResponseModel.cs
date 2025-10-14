namespace BusinessObject.DTOs.ResponseModels;

public class UserProfileResponseModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public bool Gender { get; set; }
    public string Phone { get; set; }
    public string IdNumber { get; set; }
    public string Address { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string PlaceOfIssue { get; set; }
    public string PlaceOfBirth { get; set; }
}