using FCG.Games.Domain.Enums;
using System.Text.Json.Serialization;

namespace FCG.Games.Domain.Entities;

public class Game
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Title { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string Description { get; private set; } = null!;
    public List<GameGenre> Genre { get; private set; } = [];
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public long Popularity { get; private set; }

    private Game(string title, decimal price, string description, List<GameGenre> genre)
    {
        Title = title;
        Price = price;
        Description = description;
        Genre = genre;
    }

    public static Game Create(string title, decimal price, string description, List<GameGenre> genre)
    {
        ValidateTitle(title);
        ValidatePrice(price);
        ValidateDescription(description);
        ValidateGenreList(genre);
        return new Game(title, price, description, genre);
    }
    public static Game Reconstruct(string id, string title, decimal price, string description, List<GameGenre> genre, long popularity)
    {
        var game = Create(title, price, description, genre);
        game.Id = id ?? throw new ArgumentException("Id não pode ser vazio ou nulo.");
        game.Popularity = popularity;
        return game;
    }

    #region Validators
    public static void ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título não pode ser vazio ou nulo.");
    }

    public static void ValidatePrice(decimal? price)
    {
        if (price is null or < 0)
            throw new ArgumentException("Preço não pode ser negativo.");
    }

    public static void ValidateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Descrição não pode ser vazio ou nulo.");
    }

    public static void ValidateGenreList(List<GameGenre>? genre)
    {
        if (genre == null || genre.Count == 0)
            throw new ArgumentException("Genero não pode ser vazio ou nulo.");
    }
    #endregion
    #region Updaters
    public void UpdateTitle(string title)
    {
        ValidateTitle(title);
        Title = title; 
    }

    public void UpdatePrice(decimal price)
    {
        ValidatePrice(price);
        Price = price;
    }

    public void UpdateDescription(string description)
    {
        ValidateDescription(description);
        Description = description;
    }

    public void UpdateGenre(List<GameGenre> genre)
    {
        ValidateGenreList(genre);
        Genre = genre;
    }

    public void IncrementPopularity()
    {
        Popularity++;
    }
    #endregion
}

