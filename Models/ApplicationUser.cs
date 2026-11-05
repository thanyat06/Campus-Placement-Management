
using Microsoft.AspNetCore.Identity;
using CampusPlacementManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    public AccountStatus AccountStatus { get; set; } = AccountStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Student? Student { get; set; }

    public PlacementOfficer? PlacementOfficer { get; set; }

    public CompanyHR? CompanyHR { get; set; }
}
