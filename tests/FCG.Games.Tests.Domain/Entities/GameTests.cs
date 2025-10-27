using FCG.Games.Domain.Enums;
using FCG.Games.Domain.Entities;

namespace FCG.Games.Tests.Domain.Entities;

public class GameTests
{
    [Fact]
    public void Create_ValidParameters_CreatesGame()
    {
        // Arrange & Act
        var game = Game.Create("Test Game", 0.99m, "descricao", [GameGenre.Horror]);

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
    public void Create_InvalidTitle_ArgumentException(string invalidTitle)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => Game.Create(invalidTitle, 0.99m, "descricao", [GameGenre.Horror]));
    }

    [Fact]
    public void Create_InvalidPrice_ArgumentException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => Game.Create("Test Game", -0.99m, "descricao", [GameGenre.Horror]));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_InvalidDescription_Exception(string invalidDescription)
    {
        Assert.Throws<ArgumentException>(() => Game.Create("Test Game", 0.99m, invalidDescription, [GameGenre.Horror]));
    }

    [Theory]
    [InlineData(null)]
    public void Create_InvalidGenre_Exception(List<GameGenre> invalidGenre)
    {
        Assert.Throws<ArgumentException>(() => Game.Create("Test Game", 0.99m, "descricao", invalidGenre));
    }

    [Fact]
    public void Reconstruct_ValidParameters_ReconstructObject()
    {
        // Arrange & Act
        var game = Game.Reconstruct("abc123", "Test Game", 0.99m, "descricao", [GameGenre.Horror], 100);

        // Assert
        Assert.Equal("abc123", game.Id);
    }

    [Fact]
    public void Reconstruct_NullId_ArgumentException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => Game.Reconstruct(null!, "Test Game", 0.99m, "descricao", [GameGenre.Horror], 100));
    }
}