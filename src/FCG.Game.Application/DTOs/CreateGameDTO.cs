using FCG.Game.Domain.Enums;

namespace FCG.Game.Application.DTOs;

public class CreateGameDTO
{
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public required List<GameGenre> Genre { get; set; }
}
