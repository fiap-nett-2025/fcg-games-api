using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FCG.Games.Infra.Persistence.Data;

public class FcgGameDbContext(DbContextOptions<FcgGameDbContext> options) : DbContext(options)
{
    public DbSet<Promotion> Promotions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
}
