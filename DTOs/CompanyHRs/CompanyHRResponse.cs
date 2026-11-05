
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.DTOs.CompanyHRs;

public class CompanyHRResponse
{
    public int CompanyHRId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; }
}
