namespace bot_messenger.Context;

using Microsoft.EntityFrameworkCore;

using bot_messenger.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<DocumentEmbedding> DocumentEmbeddings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentEmbedding>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Embedding)
                          .HasColumnType("vector(1536)");
                });
    }
}
