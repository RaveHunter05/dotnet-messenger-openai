namespace bot_messenger.Services;

using bot_messenger.Context;
using bot_messenger.Models;
using Microsoft.EntityFrameworkCore;

public class LocationService : ILocationService
{
    private readonly AppDbContext _context;

    public LocationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Location>> GetActiveLocationsAsync()
    {
        return await _context
            .Locations.Where(l => l.IsActive)
            .OrderBy(l => l.City)
            .ThenBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Location> GetLocationByIdAsync(int id)
    {
        return await _context
            .Locations.Include(l => l.Products.Where(p => p.Available))
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Location>> GetLocationsByCityAsync(string city)
    {
        return await _context
            .Locations.Where(l => l.City == city && l.IsActive)
            .Include(l => l.Products.Where(p => p.Available))
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Location> CreateLocationAsync(Location location)
    {
        location.CreatedAt = DateTime.UtcNow;
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return location;
    }
}
