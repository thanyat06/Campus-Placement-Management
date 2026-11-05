
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.PlacementOfficers;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/placement-officers")]
public class PlacementOfficersController : ControllerBase
{
    private readonly IPlacementOfficerService _service;

    public PlacementOfficersController(IPlacementOfficerService service)
    {
        _service = service;
    }

    // Public registration
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegistrationRequest request)
    {
        var result = await _service.RegisterAsync(request);

        if (result == null)
            return Conflict("Email already exists or college is not approved.");

        return StatusCode(201, result);
    }

    // Get all officers
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // Get pending officers
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _service.GetPendingAsync());
    }

    // Get officer by ID: SuperAdmin or the officer themselves
    [Authorize(Roles = UserRoles.SuperAdmin + "," + UserRoles.PlacementOfficer)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var officer = await _service.GetByIdAsync(id);

        if (officer == null)
            return NotFound("Placement officer not found.");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!User.IsInRole(UserRoles.SuperAdmin) &&
            officer.UserId != currentUserId)
        {
            return Forbid();
        }

        return Ok(officer);
    }

    // Approve officer
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.ApproveAsync(id, adminUserId);

        if (!result)
            return BadRequest("Officer not found, not pending, or college is not approved.");

        return Ok("Placement officer approved successfully.");
    }

    // Reject officer
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var result = await _service.RejectAsync(id);

        if (!result)
            return BadRequest("Officer not found or not pending.");

        return Ok("Placement officer rejected successfully.");
    }

    // Suspend officer
    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPut("{id:int}/suspend")]
    public async Task<IActionResult> Suspend(int id)
    {
        var result = await _service.SuspendAsync(id);

        if (!result)
            return NotFound("Placement officer not found.");

        return Ok("Placement officer suspended successfully.");
    }

    // Get logged-in officer profile
    [Authorize(Roles = UserRoles.PlacementOfficer)]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var officer = await _service.GetByUserIdAsync(userId);

        if (officer == null)
            return NotFound("Placement officer profile not found.");

        return Ok(officer);
    }
}
