// =============================================================
// File: Controllers/BookingController.cs
// Path: DunesOfArabia/Controllers/BookingController.cs
// =============================================================

using DunesOfArabia.Services;
using Microsoft.AspNetCore.Mvc;

namespace DunesOfArabia.Controllers;

[Route("api/bookings")]
public class BookingController : BaseApiController
{
    private readonly IBookingService _service;

    public BookingController(IBookingService service)
    {
        _service = service;
    }

    // GET /api/bookings  — returns only the current user's bookings
    [HttpGet]
    public async Task<IActionResult> GetMyBookings()
    {
        var result = await _service.GetByUserIdAsync(CurrentUserId!);
        return Ok(result);
    }

    // GET /api/bookings/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _service.GetByIdAsync(id);
        if (booking is null) return NotFound(new { message = $"Booking {id} not found." });

        // Only owner or admin can view
        var guard = ForbidIfNotOwner(booking.UserId);
        if (guard != null) return guard;

        return Ok(booking);
    }

    // POST /api/bookings
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
    {
        var result = await _service.CreateAsync(CurrentUserId!, dto);
        return Ok(result);
    }

    // DELETE /api/bookings/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var booking = await _service.GetByIdAsync(id);
        if (booking is null) return NotFound();

        var guard = ForbidIfNotOwner(booking.UserId);
        if (guard != null) return guard;

        await _service.CancelAsync(id);
        return NoContent();
    }
}
