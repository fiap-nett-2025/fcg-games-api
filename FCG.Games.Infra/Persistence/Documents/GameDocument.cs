using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Enums;

namespace FCG.Games.Infra.Persistence.Documents;

public class GameDocument
{
    public string? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<GameGenre> Genres { get; set; } = [];
    public int Popularity { get; set; }

    // Método para converter para a entidade de domínio
    public Game ToEntity(string id)
    {
        return Game.Reconstruct(id, Title, Price, Description, Genres, Popularity);
    }
}
