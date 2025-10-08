namespace bot_messenger.Controllers;

using bot_messenger.Models;
using Microsoft.AspNetCore.Mvc;

// Controllers/LocationsController.cs
[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveLocations()
    {
        var locations = await _locationService.GetActiveLocationsAsync();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocation(int id)
    {
        var location = await _locationService.GetLocationByIdAsync(id);
        if (location == null)
            return NotFound();
        return Ok(location);
    }

    [HttpGet("city/{city}")]
    public async Task<IActionResult> GetLocationsByCity(string city)
    {
        var locations = await _locationService.GetLocationsByCityAsync(city);
        return Ok(locations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] Location location)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdLocation = await _locationService.CreateLocationAsync(location);
        return CreatedAtAction(
            nameof(GetLocation),
            new { id = createdLocation.Id },
            createdLocation
        );
    }
}
