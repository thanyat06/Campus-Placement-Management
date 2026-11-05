
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.CompanyHRs;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/company-hrs")]
public class CompanyHRsController : ControllerBase
{
    private readonly ICompanyHRService _service;

    public CompanyHRsController(ICompanyHRService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        CompanyHRRegistrationRequest request)
    {
        var result = await _service.RegisterAsync(request);

        if (result == null)
            return Conflict("Email already exists or company is not approved.");

        return StatusCode(201, result);
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _service.GetPendingAsync());
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyHR = await _service.GetByIdAsync(id);

        if (companyHR == null)
            return NotFound("Company HR not found.");

        return Ok(companyHR);
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _service.ApproveAsync(id);

        if (!result)
            return BadRequest("Company HR not found, not pending, or company is not approved.");

        return Ok("Company HR approved successfully.");
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var result = await _service.RejectAsync(id);

        if (!result)
            return BadRequest("Company HR not found or not pending.");

        return Ok("Company HR rejected successfully.");
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPut("{id:int}/suspend")]
    public async Task<IActionResult> Suspend(int id)
    {
        var result = await _service.SuspendAsync(id);

        if (!result)
            return NotFound("Company HR not found.");

        return Ok("Company HR suspended successfully.");
    }

    [Authorize(Roles = UserRoles.CompanyHR)]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var companyHR = await _service.GetByUserIdAsync(userId);

        if (companyHR == null)
            return NotFound("Company HR profile not found.");

        return Ok(companyHR);
    }
}
