using bot_messenger.Models;

public interface IProductService
{
    Task<List<Product>> GetAvailableProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<List<Product>> GetProductsByCategoryAsync(string category);
    Task<List<Product>> GetProductsByLocationAsync(int locationId);
    Task<List<Product>> SearchProductsAsync(string searchTerm);
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
}
