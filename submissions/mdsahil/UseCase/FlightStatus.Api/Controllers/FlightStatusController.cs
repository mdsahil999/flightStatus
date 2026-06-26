using FlightStatus.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightStatus.Api.Controllers;

[ApiController]
[Route("flights")]
public class FlightStatusController(FlightStatusService service) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus([FromQuery] string? flightNumber, [FromQuery] DateTime? date)
    {
        if (string.IsNullOrWhiteSpace(flightNumber) || !date.HasValue)
        {
            return BadRequest(new { Error = "flightNumber and date are required parameters." });
        }

        var result = await service.GetFlightStatusAsync(flightNumber, date.Value);
        return Ok(result);
    }
}
