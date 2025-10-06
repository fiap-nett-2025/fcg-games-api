using FCG.Games.Domain.Enums;

namespace FCG.Games.Application.DTOs;

public class UpdatePromotionDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? DiscountPercentage { get; set; }
    public GameGenre? TargetGenre { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EnddDate { get; set; }
}
