using FCG.Game.Domain.Enums;

namespace FCG.Game.Application.DTOs;

public class PromotionDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int DiscountPercentage { get; set; }
    public required GameGenre TargetGenre { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
}
