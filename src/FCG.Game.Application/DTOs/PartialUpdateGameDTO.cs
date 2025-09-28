using FCG.Game.Domain.Enums;

namespace FCG.Game.Application.DTOs;

public class PartialUpdateGameDTO
{
    public string? Title { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public List<GameGenre>? Genre { get; set; }

}
