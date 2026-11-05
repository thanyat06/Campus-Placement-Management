
using CampusPlacementManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models;

public class Company
{
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string CompanyEmail { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Website { get; set; }

    [Required]
    [MaxLength(100)]
    public string Industry { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CompanyHR> CompanyHRs { get; set; }
        = new List<CompanyHR>();

    public ICollection<JobOpening> JobOpenings { get; set; }
        = new List<JobOpening>();
}
