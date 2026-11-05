
using CampusPlacementManagement.DTOs.Colleges;

namespace CampusPlacementManagement.Services.Interfaces;

public interface ICollegeService
{
    Task<List<CollegeResponse>> GetAllAsync();
    Task<List<CollegeResponse>> GetApprovedAsync();
    Task<CollegeResponse?> GetByIdAsync(int id);
    Task<CollegeResponse?> RegisterAsync(CollegeRequest request);
    Task<bool> UpdateAsync(int id, CollegeRequest request);
    Task<bool> UpdateStatusAsync(int id, UpdateCollegeStatusRequest request);
}
