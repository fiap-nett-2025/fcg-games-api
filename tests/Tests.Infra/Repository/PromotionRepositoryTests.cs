using Domain.Entities;
using Domain.Enums;
using Infra.Data;
using Infra.Repository;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infra.Repository;

[Trait("Category", "Repository")]
[Trait("Layer", "Infrastructure")]
[Trait("TestType", "Unit")]
public class PromotionRepositoryTests
{
    private readonly GameDbContext _context;
    private readonly PromotionRepository _repository;

    public PromotionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new GameDbContext(options);
        _repository = new PromotionRepository(_context);
    }

    [Fact]
    [Trait("Operation", "CRUD")]
    [Trait("Method", "AddAsync")]
    public async Task AddAsync_ValidPromotion_ShouldAddPromotionToDatabase()
    {
        // Assert
        var promotion = new Promotion("Action sales", "Discount in action games", 10, GameGenre.Action, DateTime.UtcNow, DateTime.UtcNow.AddDays(10));

        // Act
        await _repository.AddAsync(promotion);
        var savedPromo = await _repository.GetBy(p => p.Name == promotion.Name);

        // Assert
        Assert.NotNull(savedPromo);
        Assert.Equal("Action sales", savedPromo.Name);
    }

    [Fact]
    [Trait("Operation", "CRUD")]
    [Trait("Method", "DeleteAsync")]
    [Trait("Scenario", "Success")]
    public async Task DeleteAsync_DeleteExistingPromotion_ShouldRemovePromotionFromDatabase()
    {
        // Arrange
        var promotion = new Promotion("Action sales", "Discount in action games", 10, GameGenre.Action, DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
        await _repository.AddAsync(promotion);
        var promoToDelete = await _repository.GetBy(p => p.Name == promotion.Name);
        Assert.NotNull(promoToDelete);
        _context.ChangeTracker.Clear();

        // Act
        await _repository.DeleteAsync(promoToDelete.Id);

        // Assert
        var deletedPromotion = await _repository.GetBy(p => p.Id.Equals(promoToDelete.Id));
        Assert.Null(deletedPromotion);
    }

    [Fact]
    [Trait("Operation", "Query")]
    [Trait("Method", "GetAllAsync")]
    public async Task GetAllAsync_ShouldReturnAllPromotions()
    {
        // Arrange
        var promotions = new List<Promotion>
        {
            new Promotion("Action sales", "Discount in action games", 10, GameGenre.Action, DateTime.UtcNow, DateTime.UtcNow.AddDays(10)),
            new Promotion("RPG sales", "Discount in RPG games", 15, GameGenre.RPG, DateTime.UtcNow, DateTime.UtcNow.AddDays(20)),
            new Promotion("Halloween sales", "Discount in horror games", 20, GameGenre.Horror, DateTime.UtcNow, DateTime.UtcNow.AddDays(30))
        };
        _context.Promotions.AddRange(promotions);
        _context.SaveChanges();
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(_context.Promotions.Count(), result.Count());
        Assert.Contains(result, g => g.Name == "Action sales");
        Assert.Contains(result, g => g.Name == "RPG sales");
        Assert.Contains(result, g => g.Name == "Halloween sales");
    }

    [Fact]
    [Trait("Operation", "Query")]
    [Trait("Method", "GetBy")]
    public async Task GetBy_ByName_ShouldReturnPromotion()
    {
        // Arrange
        var promotion = new Promotion("Halloween sales", "Discount in horror games", 20, GameGenre.Horror, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        await _repository.AddAsync(promotion);

        // Act
        var result = await _repository.GetBy(g => g.Name == promotion.Name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(promotion.Name, result.Name);
    }

    [Fact]
    [Trait("Operation", "CRUD")]
    [Trait("Method", "UpdateAsync")]
    public async Task UpdateAsync_ModifyDiscountPercentage_ShouldModify()
    {
        // Arrange
        var promotion = new Promotion("Olympics sales", "Discount in sports games", 20, GameGenre.Sports, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
        await _repository.AddAsync(promotion);
        var newDiscountPercentage = 10;

        // Act
        promotion.UpdateDiscountPercentage(newDiscountPercentage);
        await _repository.UpdateAsync(promotion);

        // Assert
        var updatedPromotion = await _repository.GetBy(p => p.Id == promotion.Id);
        Assert.NotNull(updatedPromotion);
        Assert.Equal(newDiscountPercentage, updatedPromotion.DiscountPercentage);
    }
}
