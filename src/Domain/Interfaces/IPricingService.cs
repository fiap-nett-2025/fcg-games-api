using Domain.Entities;

namespace Domain.Interfaces;

public interface IPricingService
{
    decimal CalculateFinalPrice(Game game, IEnumerable<Promotion> activePromotions);
}
