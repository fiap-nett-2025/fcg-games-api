using Domain.Entities;
using Domain.Enums;

namespace Tests.Domain.Entities;

public class GameTests
{
    [Fact]
    public void CreateGame_WithValidParameters_ShouldCreateGame()
    {
        // Arrange & Act
        var game = new Game("Test Game", 0.99m, "descricao", [GameGenre.Horror]);

        // Assert
        Assert.Equal("Test Game", game.Title);
        Assert.Equal(0.99m, game.Price);
        Assert.Equal("descricao", game.Description);
        Assert.Single(game.Genre);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateGame_WithInvalidTitle_ShouldThrowException(string invalidTitle)
    {
        // Arrange, Act & Assert
        Assert.Throws<Exception>(() => new Game(invalidTitle, 0.99m, "descricao", [GameGenre.Horror]));
    }

    [Fact]
    public void CreateGame_WithInvalidPrice_ShouldCreateGame()
    {
        // Arrange, Act & Assert
        Assert.Throws<Exception>(() => new Game("Test Game", -0.99m, "descricao", [GameGenre.Horror]));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateGame_WithInvalidDescription_ShouldThrowException(string invalidDescription)
    {
        Assert.Throws<Exception>(() => new Game("Test Game", 0.99m, invalidDescription, [GameGenre.Horror]));
    }

    [Theory]
    [InlineData(null)]
    public void CreateGame_WithInvalidGenre_ShouldThrowException(List<GameGenre> invalidGenre)
    {
        Assert.Throws<Exception>(() => new Game("Test Game", 0.99m, "descricao", invalidGenre));
    }
}