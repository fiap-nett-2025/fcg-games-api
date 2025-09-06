using Domain.Enums;

namespace Application.DTOs;

public class CreatePromotionDTO
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int DiscountPercentage { get; set; }
    public required GameGenre TargetGenre { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
}
