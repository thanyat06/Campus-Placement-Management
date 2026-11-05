
using CampusPlacementManagement.DTOs.CompanyHRs;

namespace CampusPlacementManagement.Services.Interfaces;

public interface ICompanyHRService
{
    Task<CompanyHRResponse?> RegisterAsync(CompanyHRRegistrationRequest request);
    Task<List<CompanyHRResponse>> GetAllAsync();
    Task<List<CompanyHRResponse>> GetPendingAsync();
    Task<CompanyHRResponse?> GetByIdAsync(int id);
    Task<CompanyHRResponse?> GetByUserIdAsync(string userId);
    Task<bool> ApproveAsync(int id);
    Task<bool> RejectAsync(int id);
    Task<bool> SuspendAsync(int id);
}
