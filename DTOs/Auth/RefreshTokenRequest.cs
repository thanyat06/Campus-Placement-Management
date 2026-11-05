using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}