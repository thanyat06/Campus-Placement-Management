
using CampusPlacementManagement.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Colleges;

public class UpdateCollegeStatusRequest
{
    [Required]
    public ApprovalStatus Status { get; set; }
}
