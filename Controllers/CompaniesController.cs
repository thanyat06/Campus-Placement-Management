
using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.Companies;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _service;

    public CompaniesController(ICompanyService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(CompanyRequest request)
    {
        var result = await _service.RegisterAsync(request);

        if (result == null)
            return Conflict("Company email already exists.");

        return StatusCode(201, result);
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [AllowAnonymous]
    [HttpGet("approved")]
    public async Task<IActionResult> GetApproved()
    {
        return Ok(await _service.GetApprovedAsync());
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var company = await _service.GetByIdAsync(id);

        if (company == null)
            return NotFound("Company not found.");

        return Ok(company);
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CompanyRequest request)
    {
        var existing = await _service.GetByIdAsync(id);

        if (existing == null)
            return NotFound("Company not found.");

        var result = await _service.UpdateAsync(id, request);

        if (!result)
            return Conflict("Company email already exists.");

        return Ok("Company updated successfully.");
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        var result = await _service.UpdateStatusAsync(id, status);

        if (!result)
            return BadRequest("Invalid status or company not found.");

        return Ok("Company status updated successfully.");
    }
}
