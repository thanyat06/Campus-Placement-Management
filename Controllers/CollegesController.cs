
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollegesController : ControllerBase
{
    private readonly ICollegeService _collegeService;

    public CollegesController(ICollegeService collegeService)
    {
        _collegeService = collegeService;
    }

    // College registration
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(CollegeRequest request)
    {
        var result = await _collegeService.RegisterAsync(request);

        if (result == null)
            return Conflict("College code already exists.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.CollegeId },
            result);
    }

    // Get all colleges
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _collegeService.GetAllAsync());
    }

    // Get approved colleges
    [AllowAnonymous]
    [HttpGet("approved")]
    public async Task<IActionResult> GetApproved()
    {
        return Ok(await _collegeService.GetApprovedAsync());
    }

    // Get college by ID
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var college = await _collegeService.GetByIdAsync(id);

        if (college == null)
            return NotFound("College not found.");

        return Ok(college);
    }

    // Update college details
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        CollegeRequest request)
    {
        var existing = await _collegeService.GetByIdAsync(id);

        if (existing == null)
            return NotFound("College not found.");

        var result = await _collegeService.UpdateAsync(id, request);

        if (!result)
            return Conflict("College code already exists.");

        return Ok("College updated successfully.");
    }

    // Approve, reject, suspend or deactivate college
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateCollegeStatusRequest request)
    {
        var result = await _collegeService.UpdateStatusAsync(id, request);

        if (!result)
            return BadRequest("Invalid status or college not found.");

        return Ok("College status updated successfully.");
    }
}
