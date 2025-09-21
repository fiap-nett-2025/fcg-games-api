using Domain.Entities;

namespace Tests.Domain.Entities;

public class CartTests
{
    [Fact]
    public void CreateCart_ValidUser_ShouldCreateCart()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var cart = new Cart(userId);

        // Assert
        Assert.NotNull(cart);
        Assert.Equal(userId, cart.UserId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void CreateCart_InvalidUser_ShouldThrowArgumentException(string userId)
    {
        // Arrange, act & assert
        Assert.Throws<ArgumentException>(() => new Cart(userId));        
    }

    [Fact]
    public void AddGame_GameNotInCart_ShouldAddGame()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var gameId = 1;
        var cart = new Cart(userId);

        // Act
        cart.AddGame(gameId);

        // Assert
        Assert.Contains(gameId, cart.GameIds);
    }

    [Fact]
    public void AddGame_GameInCart_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var gameId = 3;
        var cart = new Cart(userId);
        cart.AddGame(gameId);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => cart.AddGame(gameId));
    }

    [Fact]
    public void RemoveGame_GameInCart_ShouldRemoveGame()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var gameId = 1;
        var cart = new Cart(userId);
        cart.AddGame(gameId);

        // Act
        cart.RemoveGame(gameId);

        // Assert
        Assert.DoesNotContain(gameId, cart.GameIds);
    }

    [Fact]
    public void RemoveGame_GameNotInCart_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var gameId = 1;
        var cart = new Cart(userId);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => cart.RemoveGame(gameId));
    }

    [Fact]
    public void ClearCart_ShouldClearCart()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var cart = new Cart(userId);
        cart.AddGame(1);
        
        // Act
        cart.ClearCart();

        // Assert
        Assert.Empty(cart.GameIds);
    }
}
