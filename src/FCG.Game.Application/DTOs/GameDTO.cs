using FCG.Game.Domain.Enums;

namespace FCG.Game.Application.DTOs;

public class GameDTO
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public  required string Description { get; set; }
    public required List<GameGenre> Genre { get; set; }
    public long Popularity { get; set; }
}
