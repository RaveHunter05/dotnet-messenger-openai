namespace bot_messenger.Models;

public class Location
{
    public int Id { get; set; }

    public string Name { get; set; } // Ej: "Sucursal Centro", "Tienda Online"

    public string Address { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }

    public bool IsActive { get; set; } = true;

    // Relación: una ubicación tiene muchos productos
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
