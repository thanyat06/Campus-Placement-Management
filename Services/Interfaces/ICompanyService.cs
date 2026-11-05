
using CampusPlacementManagement.DTOs.Companies;

namespace CampusPlacementManagement.Services.Interfaces;

public interface ICompanyService
{
    Task<List<CompanyResponse>> GetAllAsync();
    Task<List<CompanyResponse>> GetApprovedAsync();
    Task<CompanyResponse?> GetByIdAsync(int id);
    Task<CompanyResponse?> RegisterAsync(CompanyRequest request);
    Task<bool> UpdateAsync(int id, CompanyRequest request);
    Task<bool> UpdateStatusAsync(int id, string status);
}
