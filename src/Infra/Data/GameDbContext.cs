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
            .IsRequired()
            .HasConversion(
                v => string.Join(",", v.Select(g => g.ToString())), // List to string
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(g => (GameGenre)Enum.Parse(typeof(GameGenre), g))
                     .ToList() // String to List
            );
        });
    }
}
