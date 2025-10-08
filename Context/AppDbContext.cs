namespace bot_messenger.Context;

using bot_messenger.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<DocumentEmbedding> DocumentEmbeddings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);

            entity.Property(p => p.Price).HasColumnType("decimal(10,2)");

            entity.Property(p => p.Color).HasMaxLength(50);

            entity.Property(p => p.Category).HasMaxLength(100);

            // Relación con Location
            entity
                .HasOne(p => p.Location)
                .WithMany(l => l.Products)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para búsquedas rápidas
            entity.HasIndex(p => p.Category);
            entity.HasIndex(p => p.Available);
            entity.HasIndex(p => p.Price);
            entity.HasIndex(p => new { p.Category, p.Available });
        });

        // Configuración de Location
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Name).IsRequired().HasMaxLength(100);

            entity.Property(l => l.Address).IsRequired().HasMaxLength(100);

            entity.Property(l => l.City).HasMaxLength(50);

            entity.Property(l => l.State).HasMaxLength(50);

            entity.Property(l => l.Phone).HasMaxLength(20);

            entity.Property(l => l.Email).HasMaxLength(100);

            entity.HasIndex(l => l.City);
            entity.HasIndex(l => l.IsActive);
        });

        modelBuilder.Entity<DocumentEmbedding>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Embedding).HasColumnType("vector(1536)");
            entity.Property(de => de.Metadata).HasColumnType("jsonb");

            entity.HasIndex(de => de.SourceTable);
        });
    }
}
