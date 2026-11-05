
using CampusPlacementManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models;

public class PlacementOfficer
{
    public int PlacementOfficerId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int CollegeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public ApplicationUser? ApprovedByUser { get; set; }

    public College College { get; set; } = null!;

    public ICollection<Student> Students { get; set; }
        = new List<Student>();
}
