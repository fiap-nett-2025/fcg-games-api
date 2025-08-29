using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data;

public class GameDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("games");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasIndex(e => e.Title).IsUnique();
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Genre)
                .HasMaxLength(500)
                .IsRequired()
                .HasConversion(
                     v => string.Join(',', v), // Convert List<GameGenre> to string for storage
                     v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(gg => Enum.Parse<GameGenre>(gg)).ToList() // Convert string back to List<GameGenre>
                );
        });
    }
}
