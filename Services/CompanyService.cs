
using CampusPlacementManagement.DTOs.Companies;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services.Interfaces;

namespace CampusPlacementManagement.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;

    public CompanyService(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CompanyResponse>> GetAllAsync()
    {
        var companies = await _repository.GetAllAsync();
        return companies.Select(MapToResponse).ToList();
    }

    public async Task<List<CompanyResponse>> GetApprovedAsync()
    {
        var companies = await _repository.GetApprovedAsync();
        return companies.Select(MapToResponse).ToList();
    }

    public async Task<CompanyResponse?> GetByIdAsync(int id)
    {
        var company = await _repository.GetByIdAsync(id);

        return company == null ? null : MapToResponse(company);
    }

    public async Task<CompanyResponse?> RegisterAsync(CompanyRequest request)
    {
        var existing = await _repository.GetByEmailAsync(request.CompanyEmail);

        if (existing != null)
            return null;

        var company = new Company
        {
            CompanyName = request.CompanyName,
            CompanyEmail = request.CompanyEmail,
            Website = request.Website,
            Industry = request.Industry,
            Address = request.Address,
            Status = ApprovalStatus.Pending
        };

        await _repository.AddAsync(company);

        return MapToResponse(company);
    }

    public async Task<bool> UpdateAsync(int id, CompanyRequest request)
    {
        var company = await _repository.GetByIdAsync(id);

        if (company == null)
            return false;

        var existing = await _repository.GetByEmailAsync(request.CompanyEmail);

        if (existing != null && existing.CompanyId != id)
            return false;

        company.CompanyName = request.CompanyName;
        company.CompanyEmail = request.CompanyEmail;
        company.Website = request.Website;
        company.Industry = request.Industry;
        company.Address = request.Address;

        await _repository.UpdateAsync(company);

        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        if (!Enum.TryParse<ApprovalStatus>(status, true, out var newStatus) ||
            !Enum.IsDefined(typeof(ApprovalStatus), newStatus))
            return false;

        var company = await _repository.GetByIdAsync(id);

        if (company == null)
            return false;

        company.Status = newStatus;

        await _repository.UpdateAsync(company);

        return true;
    }

    private static CompanyResponse MapToResponse(Company company)
    {
        return new CompanyResponse
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            CompanyEmail = company.CompanyEmail,
            Website = company.Website,
            Industry = company.Industry,
            Address = company.Address,
            Status = company.Status,
            CreatedAt = company.CreatedAt
        };
    }
}
