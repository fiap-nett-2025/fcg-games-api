using Domain.Enums;

namespace Domain.Entities;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public List<GameGenre> Genre { get; set; }

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
    private Game() { } // For EF Core

    public static void ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new Exception("Título não pode ser vazio ou nulo.");
    }

    public static void ValidatePrice(decimal? price)
    {
        if (price is null or < 0)
            throw new Exception("Preço não pode ser negativo.");
    }

    public static void ValidateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new Exception("Descrição não pode ser vazio ou nulo.");
    }

    public static void ValidateGenreList(List<GameGenre>? genre)
    {
        if (genre == null || genre.Count == 0)
            throw new Exception("Genero não pode ser vazio ou nulo.");
    }
}

