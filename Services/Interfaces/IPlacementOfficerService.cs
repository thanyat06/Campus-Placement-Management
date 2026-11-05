
using CampusPlacementManagement.DTOs.PlacementOfficers;

namespace CampusPlacementManagement.Services.Interfaces;

public interface IPlacementOfficerService
{
    Task<PlacementOfficerResponse?> RegisterAsync(RegistrationRequest request);
    Task<List<PlacementOfficerResponse>> GetAllAsync();
    Task<List<PlacementOfficerResponse>> GetPendingAsync();
    Task<PlacementOfficerResponse?> GetByIdAsync(int id);
    Task<PlacementOfficerResponse?> GetByUserIdAsync(string userId);
    Task<bool> ApproveAsync(int id, string adminUserId);
    Task<bool> RejectAsync(int id);
    Task<bool> SuspendAsync(int id);
}
