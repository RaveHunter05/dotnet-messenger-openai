using bot_messenger.Models;

public interface ILocationService
{
    Task<List<Location>> GetActiveLocationsAsync();
    Task<Location> GetLocationByIdAsync(int id);
    Task<List<Location>> GetLocationsByCityAsync(string city);
    Task<Location> CreateLocationAsync(Location location);
}
