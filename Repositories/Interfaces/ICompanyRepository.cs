
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<List<Company>> GetAllAsync();
    Task<List<Company>> GetApprovedAsync();
    Task<Company?> GetByIdAsync(int id);
    Task<Company?> GetByEmailAsync(string email);
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
}
