
using CampusPlacementManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models;

public class CompanyHR
{
    public int CompanyHRId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int CompanyId { get; set; }

    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    public ApplicationUser User { get; set; } = null!;

    public Company Company { get; set; } = null!;

    public ICollection<JobOpening> JobOpenings { get; set; }
        = new List<JobOpening>();
}
