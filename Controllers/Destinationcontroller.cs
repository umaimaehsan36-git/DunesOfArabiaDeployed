// =============================================================
// File: Controllers/DestinationController.cs
// Path: DunesOfArabia/Controllers/DestinationController.cs
// =============================================================

using DunesOfArabia.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DunesOfArabia.Controllers;

[Route("api/destinations")]
public class DestinationController : BaseApiController
{
    private readonly IDestinationService _service;

    public DestinationController(IDestinationService service)
    {
        _service = service;
    }

    // GET /api/destinations  — public, no login needed
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // GET /api/destinations/{id}  — public
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound(new { message = $"Destination {id} not found." }) : Ok(result);
    }

    // POST /api/destinations  — Admin only
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateDestinationDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    // PUT /api/destinations/{id}  — Admin only
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDestinationDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    // DELETE /api/destinations/{id}  — Admin only
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
