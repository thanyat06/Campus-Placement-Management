
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CompanyHRRepository : ICompanyHRRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyHRRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CompanyHR>> GetAllAsync()
    {
        return await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .ToListAsync();
    }

    public async Task<List<CompanyHR>> GetPendingAsync()
    {
        return await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .Where(h => h.Status == ApprovalStatus.Pending)
            .ToListAsync();
    }

    public async Task<CompanyHR?> GetByIdAsync(int id)
    {
        return await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .FirstOrDefaultAsync(h => h.CompanyHRId == id);
    }

    public async Task<CompanyHR?> GetByUserIdAsync(string userId)
    {
        return await _context.CompanyHRs
            .Include(h => h.User)
            .Include(h => h.Company)
            .FirstOrDefaultAsync(h => h.UserId == userId);
    }

    public async Task AddAsync(CompanyHR companyHR)
    {
        _context.CompanyHRs.Add(companyHR);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CompanyHR companyHR)
    {
        _context.CompanyHRs.Update(companyHR);
        await _context.SaveChangesAsync();
    }
}
