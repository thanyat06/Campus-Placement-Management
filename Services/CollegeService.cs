
using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services;

public class CollegeService : ICollegeService
{
    private readonly ICollegeRepository _collegeRepository;

    public CollegeService(ICollegeRepository collegeRepository)
    {
        _collegeRepository = collegeRepository;
    }

    public async Task<List<CollegeResponse>> GetAllAsync()
    {
        var colleges = await _collegeRepository.GetAllAsync();
        return colleges.Select(MapToResponse).ToList();
    }

    public async Task<List<CollegeResponse>> GetApprovedAsync()
    {
        var colleges = await _collegeRepository.GetApprovedAsync();
        return colleges.Select(MapToResponse).ToList();
    }

    public async Task<CollegeResponse?> GetByIdAsync(int id)
    {
        var college = await _collegeRepository.GetByIdAsync(id);

        return college == null ? null : MapToResponse(college);
    }

    public async Task<CollegeResponse?> RegisterAsync(CollegeRequest request)
    {
        var existingCollege = await _collegeRepository.GetByCodeAsync(
            request.CollegeCode);

        if (existingCollege != null)
            return null;

        var college = new College
        {
            CollegeName = request.CollegeName,
            CollegeCode = request.CollegeCode,
            UniversityName = request.UniversityName,
            OfficialEmail = request.OfficialEmail,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            Website = request.Website,
            Status = ApprovalStatus.Pending
        };

        await _collegeRepository.AddAsync(college);

        return MapToResponse(college);
    }

    public async Task<bool> UpdateAsync(int id, CollegeRequest request)
    {
        var college = await _collegeRepository.GetByIdAsync(id);

        if (college == null)
            return false;

        var existingCollege = await _collegeRepository.GetByCodeAsync(
            request.CollegeCode);

        if (existingCollege != null &&
            existingCollege.CollegeId != id)
            return false;

        college.CollegeName = request.CollegeName;
        college.CollegeCode = request.CollegeCode;
        college.UniversityName = request.UniversityName;
        college.OfficialEmail = request.OfficialEmail;
        college.PhoneNumber = request.PhoneNumber;
        college.Address = request.Address;
        college.Website = request.Website;

        await _collegeRepository.UpdateAsync(college);

        return true;
    }

    public async Task<bool> UpdateStatusAsync(
        int id,
        UpdateCollegeStatusRequest request)
    {
        if (!Enum.IsDefined(typeof(ApprovalStatus), request.Status))
            return false;

        var college = await _collegeRepository.GetByIdAsync(id);

        if (college == null)
            return false;

        college.Status = request.Status;

        await _collegeRepository.UpdateAsync(college);

        return true;
    }

    private static CollegeResponse MapToResponse(College college)
    {
        return new CollegeResponse
        {
            CollegeId = college.CollegeId,
            CollegeName = college.CollegeName,
            CollegeCode = college.CollegeCode,
            UniversityName = college.UniversityName,
            OfficialEmail = college.OfficialEmail,
            PhoneNumber = college.PhoneNumber,
            Address = college.Address,
            Website = college.Website,
            Status = college.Status,
            CreatedAt = college.CreatedAt
        };
    }
}
