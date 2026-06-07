// =============================================================
// File: Controllers/ActivityController.cs
// Path: DunesOfArabia/Controllers/ActivityController.cs
// =============================================================

using DunesOfArabia.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DunesOfArabia.Controllers;

[Route("api/activities")]
public class ActivityController : BaseApiController
{
    private readonly IActivityService _service;
    public ActivityController(IActivityService service) { _service = service; }

    // GET /api/activities  — public
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    // GET /api/activities/{id}  — public
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // POST /api/activities  — Admin only
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateActivityDto dto)
        => Ok(await _service.CreateAsync(dto));

    // PUT /api/activities/{id}  — Admin only
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateActivityDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    // DELETE /api/activities/{id}  — Admin only
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
