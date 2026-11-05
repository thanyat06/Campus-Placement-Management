
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories.Interfaces;

public interface ICollegeRepository
{
    Task<List<College>> GetAllAsync();
    Task<List<College>> GetApprovedAsync();
    Task<College?> GetByIdAsync(int id);
    Task<College?> GetByCodeAsync(string collegeCode);
    Task AddAsync(College college);
    Task UpdateAsync(College college);
}
