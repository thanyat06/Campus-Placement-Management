
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.DTOs.Companies;

public class CompanyResponse
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string Industry { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
