

namespace BusinessObject.DTOs.ResponseModels
{
    public class GroupBasicReponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public string? GovernancePolicy { get; set; }
        public bool IsActive { get; set; }
    }
}
