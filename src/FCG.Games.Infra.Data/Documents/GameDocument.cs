using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Enums;

namespace FCG.Games.Infra.Data.Documents;

public class GameDocument
{
    public string? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<GameGenre> Genre { get; set; }
    public int Popularity { get; set; }

    // Método para converter para a entidade de domínio
    public Game ToEntity(string id)
    {
        return Game.Reconstruct(id, Title, Price, Description, Genre, Popularity);
    }
}
