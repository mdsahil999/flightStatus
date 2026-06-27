using FlightStatus.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightStatus.Api.Controllers;

[ApiController]
[Route("flights")]
public class FlightStatusController : ControllerBase
{
    private readonly FlightStatusService _service;

    public FlightStatusController(FlightStatusService service)
    {
        _service = service;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus([FromQuery] string? flightNumber, [FromQuery] DateTime? date)
    {
        if (string.IsNullOrWhiteSpace(flightNumber) || !date.HasValue)
        {
            return BadRequest(new { Error = "flightNumber and date are required parameters." });
        }

        var result = await _service.GetFlightStatusAsync(flightNumber, date.Value);
        return Ok(result);
    }
}
