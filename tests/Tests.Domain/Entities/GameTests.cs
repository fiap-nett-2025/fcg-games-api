using Domain.Entities;
using Domain.Enums;

namespace Tests.Domain.Entities;

public class GameTests
{
    [Fact]
    public void Constructor_ValidParameters_CreatesGame()
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
    public void Constructor_InvalidTitle_Exception(string invalidTitle)
    {
        // Arrange, Act & Assert
        Assert.Throws<Exception>(() => new Game(invalidTitle, 0.99m, "descricao", [GameGenre.Horror]));
    }

    [Fact]
    public void Constructor_InvalidPrice_CreatesGame()
    {
        // Arrange, Act & Assert
        Assert.Throws<Exception>(() => new Game("Test Game", -0.99m, "descricao", [GameGenre.Horror]));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_InvalidDescription_Exception(string invalidDescription)
    {
        Assert.Throws<Exception>(() => new Game("Test Game", 0.99m, invalidDescription, [GameGenre.Horror]));
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_InvalidGenre_Exception(List<GameGenre> invalidGenre)
    {
        Assert.Throws<Exception>(() => new Game("Test Game", 0.99m, "descricao", invalidGenre));
    }
}