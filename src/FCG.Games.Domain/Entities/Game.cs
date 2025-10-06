using FCG.Games.Domain.Enums;

namespace FCG.Games.Domain.Entities;

public class Game
{
    public string Id { get; set; } 
    public string Title { get;  set; } = null!;
    public decimal Price { get;  set; }
    public string Description { get;  set; } = null!;
    public List<GameGenre> Genre { get;  set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long Popularity { get; set; }

    public Game(string title, decimal price, string description, List<GameGenre> genre)
    {
        ValidateTitle(title);
        ValidatePrice(price);
        ValidateDescription(description);
        ValidateGenreList(genre);

        Title = title;
        Price = price;
        Description = description;
        Genre = genre;
    }

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
}

