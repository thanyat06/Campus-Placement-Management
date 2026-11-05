
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class CompanyHRService : ICompanyHRService
{
    private readonly ICompanyHRRepository _repository;
    private readonly ICompanyRepository _companyRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public CompanyHRService(
        ICompanyHRRepository repository,
        ICompanyRepository companyRepository,
        UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _companyRepository = companyRepository;
        _userManager = userManager;
    }

    public async Task<CompanyHRResponse?> RegisterAsync(
        CompanyHRRegistrationRequest request)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);

        if (company == null || company.Status != ApprovalStatus.Approved)
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

        var companyHR = new CompanyHR
        {
            UserId = user.Id,
            CompanyId = request.CompanyId,
            Status = ApprovalStatus.Pending
        };

        await _repository.AddAsync(companyHR);

        return MapToResponse(companyHR, user, company.CompanyName);
    }

    public async Task<List<CompanyHRResponse>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();

        return list.Select(h =>
            MapToResponse(h, h.User, h.Company.CompanyName)).ToList();
    }

    public async Task<List<CompanyHRResponse>> GetPendingAsync()
    {
        var list = await _repository.GetPendingAsync();

        return list.Select(h =>
            MapToResponse(h, h.User, h.Company.CompanyName)).ToList();
    }

    public async Task<CompanyHRResponse?> GetByIdAsync(int id)
    {
        var companyHR = await _repository.GetByIdAsync(id);

        if (companyHR == null)
            return null;

        return MapToResponse(
            companyHR, companyHR.User, companyHR.Company.CompanyName);
    }

    public async Task<CompanyHRResponse?> GetByUserIdAsync(string userId)
    {
        var companyHR = await _repository.GetByUserIdAsync(userId);

        if (companyHR == null)
            return null;

        return MapToResponse(
            companyHR, companyHR.User, companyHR.Company.CompanyName);
    }

    public async Task<bool> ApproveAsync(int id)
    {
        var companyHR = await _repository.GetByIdAsync(id);

        if (companyHR == null || companyHR.Status != ApprovalStatus.Pending)
            return false;

        if (companyHR.Company.Status != ApprovalStatus.Approved)
            return false;

        companyHR.Status = ApprovalStatus.Approved;
        companyHR.User.AccountStatus = AccountStatus.Approved;
        companyHR.User.UpdatedAt = DateTime.UtcNow;

        var roleResult = await _userManager.AddToRoleAsync(
            companyHR.User, UserRoles.CompanyHR);

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }

        await _userManager.UpdateAsync(companyHR.User);
        await _repository.UpdateAsync(companyHR);

        return true;
    }

    public async Task<bool> RejectAsync(int id)
    {
        var companyHR = await _repository.GetByIdAsync(id);

        if (companyHR == null || companyHR.Status != ApprovalStatus.Pending)
            return false;

        companyHR.Status = ApprovalStatus.Rejected;
        companyHR.User.AccountStatus = AccountStatus.Rejected;
        companyHR.User.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(companyHR.User);
        await _repository.UpdateAsync(companyHR);

        return true;
    }

    public async Task<bool> SuspendAsync(int id)
    {
        var companyHR = await _repository.GetByIdAsync(id);

        if (companyHR == null)
            return false;

        companyHR.Status = ApprovalStatus.Suspended;
        companyHR.User.AccountStatus = AccountStatus.Suspended;
        companyHR.User.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(companyHR.User);
        await _repository.UpdateAsync(companyHR);

        return true;
    }

    private static CompanyHRResponse MapToResponse(
        CompanyHR companyHR,
        ApplicationUser user,
        string companyName)
    {
        return new CompanyHRResponse
        {
            CompanyHRId = companyHR.CompanyHRId,
            UserId = companyHR.UserId,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CompanyId = companyHR.CompanyId,
            CompanyName = companyName,
            Status = companyHR.Status
        };
    }
}
