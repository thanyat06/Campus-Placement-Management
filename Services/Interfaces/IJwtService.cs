
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
}
