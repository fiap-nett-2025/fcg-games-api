using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infra.Data;

public class GameDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Cart> Carts { get; set; }
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
                     .ToList(), // String to List
                new ValueComparer<List<GameGenre>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("promotions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.DiscountPercentage)
                .IsRequired()
                .HasColumnType("decimal(5,2)");
            entity.Property(e => e.TargetGenre)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (GameGenre)Enum.Parse(typeof(GameGenre), v)
                );
            entity.Property(e => e.StartDate)
                .IsRequired();
            entity.Property(e => e.EndDate)
                .IsRequired();
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("carts");
            entity.HasKey(e => e.UserId);
            entity.Property(e =>e.UserId)
                .IsRequired()
                .ValueGeneratedNever();
            entity.Property(e => e.GameIds)
                .IsRequired()
                .HasConversion(
                     v => string.Join(",", v),
                     v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToList(),
                     new ValueComparer<List<int>>(
                         (c1, c2) => c1.SequenceEqual(c2),
                         c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                         c => c.ToList()
                     )
                )
                .HasColumnType("nvarchar(1000)");
        });
    }
}
