
using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class PlacementOfficerService : IPlacementOfficerService
{
    private readonly IPlacementOfficerRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICollegeRepository _collegeRepository;

    public PlacementOfficerService(
        IPlacementOfficerRepository repository,
        UserManager<ApplicationUser> userManager,
        ICollegeRepository collegeRepository)
    {
        _repository = repository;
        _userManager = userManager;
        _collegeRepository = collegeRepository;
    }

    public async Task<PlacementOfficerResponse?> RegisterAsync(
        RegistrationRequest request)
    {
        var college = await _collegeRepository.GetByIdAsync(request.CollegeId);

        if (college == null || college.Status != ApprovalStatus.Approved)
            return null;

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
            return null;

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            AccountStatus = AccountStatus.Pending
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var officer = new PlacementOfficer
        {
            UserId = user.Id,
            CollegeId = request.CollegeId,
            Designation = request.Designation,
            Status = ApprovalStatus.Pending
        };

        await _repository.AddAsync(officer);

        return MapToResponse(officer, user, college.CollegeName);
    }

    public async Task<List<PlacementOfficerResponse>> GetAllAsync()
    {
        var officers = await _repository.GetAllAsync();

        return officers.Select(p => MapToResponse(
            p, p.User, p.College.CollegeName)).ToList();
    }

    public async Task<List<PlacementOfficerResponse>> GetPendingAsync()
    {
        var officers = await _repository.GetPendingAsync();

        return officers.Select(p => MapToResponse(
            p, p.User, p.College.CollegeName)).ToList();
    }

    public async Task<PlacementOfficerResponse?> GetByIdAsync(int id)
    {
        var officer = await _repository.GetByIdAsync(id);

        if (officer == null)
            return null;

        return MapToResponse(
            officer, officer.User, officer.College.CollegeName);
    }

    public async Task<PlacementOfficerResponse?> GetByUserIdAsync(string userId)
    {
        var officer = await _repository.GetByUserIdAsync(userId);

        if (officer == null)
            return null;

        return MapToResponse(
            officer, officer.User, officer.College.CollegeName);
    }

    public async Task<bool> ApproveAsync(int id, string adminUserId)
    {
        var officer = await _repository.GetByIdAsync(id);

        if (officer == null || officer.Status != ApprovalStatus.Pending)
            return false;

        var college = await _collegeRepository.GetByIdAsync(officer.CollegeId);

        if (college == null || college.Status != ApprovalStatus.Approved)
            return false;

        officer.Status = ApprovalStatus.Approved;
        officer.ApprovedBy = adminUserId;
        officer.ApprovedAt = DateTime.UtcNow;

        officer.User.AccountStatus = AccountStatus.Approved;
        officer.User.UpdatedAt = DateTime.UtcNow;

        var roleResult = await _userManager.AddToRoleAsync(
            officer.User, UserRoles.PlacementOfficer);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }

        await _userManager.UpdateAsync(officer.User);
        await _repository.UpdateAsync(officer);

        return true;
    }

    public async Task<bool> RejectAsync(int id)
    {
        var officer = await _repository.GetByIdAsync(id);

        if (officer == null || officer.Status != ApprovalStatus.Pending)
            return false;

        officer.Status = ApprovalStatus.Rejected;
        officer.User.AccountStatus = AccountStatus.Rejected;
        officer.User.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(officer.User);
        await _repository.UpdateAsync(officer);

        return true;
    }

    public async Task<bool> SuspendAsync(int id)
    {
        var officer = await _repository.GetByIdAsync(id);

        if (officer == null)
            return false;

        officer.Status = ApprovalStatus.Suspended;
        officer.User.AccountStatus = AccountStatus.Suspended;
        officer.User.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(officer.User);
        await _repository.UpdateAsync(officer);

        return true;
    }

    private static PlacementOfficerResponse MapToResponse(
        PlacementOfficer officer,
        ApplicationUser user,
        string collegeName)
    {
        return new PlacementOfficerResponse
        {
            PlacementOfficerId = officer.PlacementOfficerId,
            UserId = officer.UserId,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CollegeId = officer.CollegeId,
            CollegeName = collegeName,
            Designation = officer.Designation,
            Status = officer.Status,
            ApprovedBy = officer.ApprovedBy,
            ApprovedAt = officer.ApprovedAt
        };
    }
}
