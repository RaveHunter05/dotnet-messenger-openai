namespace bot_messenger.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool Available { get; set; } = true;

    public decimal Price { get; set; }

    public string Color { get; set; }

    public string Category { get; set; }

    // Relación con Location (muchos productos pueden estar en una ubicación)
    public int LocationId { get; set; }
    public virtual Location Location { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
