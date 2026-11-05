
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Models.Enums;
using CampusPlacementManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class PlacementOfficerRepository : IPlacementOfficerRepository
{
    private readonly ApplicationDbContext _context;

    public PlacementOfficerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlacementOfficer>> GetAllAsync()
    {
        return await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .ToListAsync();
    }

    public async Task<List<PlacementOfficer>> GetPendingAsync()
    {
        return await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .Where(p => p.Status == ApprovalStatus.Pending)
            .ToListAsync();
    }

    public async Task<PlacementOfficer?> GetByIdAsync(int id)
    {
        return await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .FirstOrDefaultAsync(p => p.PlacementOfficerId == id);
    }

    public async Task<PlacementOfficer?> GetByUserIdAsync(string userId)
    {
        return await _context.PlacementOfficers
            .Include(p => p.User)
            .Include(p => p.College)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task AddAsync(PlacementOfficer officer)
    {
        _context.PlacementOfficers.Add(officer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PlacementOfficer officer)
    {
        _context.PlacementOfficers.Update(officer);
        await _context.SaveChangesAsync();
    }
}
