using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Enums;

namespace FCG.Games.Tests.Domain.Entities;

public class PromotionTests
{
    [Fact]
    public void CreatePromotion_WithValidParameters_ShouldCreatePromotion()
    {
        // Arrange
        var name = "RPG Sale";
        var description = "Discounts on RPG games";
        var discountPercentage = 20;
        var targetGenre = GameGenre.RPG;
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(30);
        // Act
        var promotion = new Promotion(name, description, discountPercentage, targetGenre, startDate, endDate);
        // Assert
        Assert.Equal(name, promotion.Name);
        Assert.Equal(description, promotion.Description);
        Assert.Equal(discountPercentage, promotion.DiscountPercentage);
        Assert.Equal(targetGenre, promotion.TargetGenre);
        Assert.Equal(startDate, promotion.StartDate);
        Assert.Equal(endDate, promotion.EndDate);
    }

    [Fact]
    public void CreatePromotion_WithInvalidDiscountPercentage_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var name = "FPS Sale";
        var description = "Discounts on FPS games";
        var discountPercentage = 150; // Invalid percentage
        var targetGenre = GameGenre.RPG;
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(30);
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Promotion(name, description, discountPercentage, targetGenre, startDate, endDate));
    }

    [Fact]
    public void ApplyDiscount_ShouldApplyDiscount_WhenGameGenreMatches()
    {
        // Arrange
        var promotion = new Promotion("RPG Sale", "Desconto RPG", 20, GameGenre.RPG, DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
        var game = new Game("Final Fantasy", 100m, "descrição", [GameGenre.RPG]);

        // Act
        var discountedPrice = promotion.CalculateDiscountedPrice(game);

        // Assert
        Assert.Equal(80m, discountedPrice);
    }

    [Fact]
    public void ApplyDiscount_ShouldNotApplyDiscount_WhenGameGenreDoesNotMatch()
    {
        // Arrange
        var promotion = new Promotion("RPG Sale", "Desconto RPG",  20, GameGenre.RPG, DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
        var game = new Game("FIFA", 100m, "Descrição", [GameGenre.Sports]);

        // Act
        var discountedPrice = promotion.CalculateDiscountedPrice(game);

        // Assert
        Assert.Equal(100m, discountedPrice);
    }

    [Fact]
    public void ApplyDiscount_ShouldNotApplyDiscount_OutsidePromotionPeriod()
    {
        // Arrange
        var promotion = new Promotion("RPG Sale", "Desconto RPG", 20, GameGenre.RPG, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow.AddDays(-10));
        var game = new Game("Final Fantasy", 100m, "Descrição", [GameGenre.RPG]);

        // Act
        var discountedPrice = promotion.CalculateDiscountedPrice(game);

        // Assert
        Assert.Equal(100m, discountedPrice);
    }
}
