// =============================================================
// File: Controllers/ItineraryController.cs
// Path: DunesOfArabia/Controllers/ItineraryController.cs
// =============================================================

using DunesOfArabia.Services;
using Microsoft.AspNetCore.Mvc;

namespace DunesOfArabia.Controllers;

[Route("api/itineraries")]
public class ItineraryController : BaseApiController
{
    private readonly IItineraryService _service;
    public ItineraryController(IItineraryService service) { _service = service; }

    // GET /api/itineraries  — returns only the current user's itineraries
    [HttpGet]
    public async Task<IActionResult> GetMine()
        => Ok(await _service.GetByUserIdAsync(CurrentUserId!));

    // GET /api/itineraries/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound();
        var guard = ForbidIfNotOwner(result.UserId);
        if (guard != null) return guard;
        return Ok(result);
    }

    // POST /api/itineraries
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItineraryDto dto)
        => Ok(await _service.CreateAsync(CurrentUserId!, dto));

    // PUT /api/itineraries/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateItineraryDto dto)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing is null) return NotFound();
        var guard = ForbidIfNotOwner(existing.UserId);
        if (guard != null) return guard;
        var result = await _service.UpdateAsync(id, dto);
        return Ok(result);
    }

    // DELETE /api/itineraries/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing is null) return NotFound();
        var guard = ForbidIfNotOwner(existing.UserId);
        if (guard != null) return guard;
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
