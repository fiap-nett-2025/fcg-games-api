using Domain.Enums;

namespace Application.DTOs;

public class UpdateGameDTO
{
    public string? Title { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public List<GameGenre>? Genre { get; set; }

}
