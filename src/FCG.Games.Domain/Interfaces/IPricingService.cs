using FCG.Games.Domain.Entities;

namespace FCG.Games.Domain.Interfaces;

public interface IPricingService
{
    decimal CalculateFinalPrice(Game game, IEnumerable<Promotion> activePromotions);
}
