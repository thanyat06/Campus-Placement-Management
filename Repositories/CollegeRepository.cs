
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CollegeRepository : ICollegeRepository
{
    private readonly ApplicationDbContext _context;

    public CollegeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<College>> GetAllAsync()
    {
        return await _context.Colleges
            .OrderBy(c => c.CollegeName)
            .ToListAsync();
    }

    public async Task<List<College>> GetApprovedAsync()
    {
        return await _context.Colleges
            .Where(c => c.Status == ApprovalStatus.Approved)
            .OrderBy(c => c.CollegeName)
            .ToListAsync();
    }

    public async Task<College?> GetByIdAsync(int id)
    {
        return await _context.Colleges.FindAsync(id);
    }

    public async Task<College?> GetByCodeAsync(string collegeCode)
    {
        return await _context.Colleges
            .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);
    }

    public async Task AddAsync(College college)
    {
        _context.Colleges.Add(college);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(College college)
    {
        _context.Colleges.Update(college);
        await _context.SaveChangesAsync();
    }
}
