namespace bot_messenger.Services;

// Services/ProductService.cs
using bot_messenger.Context;
using bot_messenger.Models;
using Microsoft.EntityFrameworkCore;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAvailableProductsAsync()
    {
        return await _context
            .Products.Include(p => p.Location)
            .Where(p => p.Available && p.Location.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _context
            .Products.Include(p => p.Location)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _context
            .Products.Include(p => p.Location)
            .Where(p => p.Category == category && p.Available && p.Location.IsActive)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByLocationAsync(int locationId)
    {
        return await _context
            .Products.Include(p => p.Location)
            .Where(p => p.LocationId == locationId && p.Available)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<List<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _context
            .Products.Include(p => p.Location)
            .Where(p =>
                p.Available
                && p.Location.IsActive
                && (
                    p.Name.Contains(searchTerm)
                    || p.Category.Contains(searchTerm)
                    || p.Color.Contains(searchTerm)
                )
            )
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}
