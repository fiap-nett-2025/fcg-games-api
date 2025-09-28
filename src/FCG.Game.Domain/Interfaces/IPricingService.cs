using FCG.Game.Domain.Entities;

namespace FCG.Game.Domain.Interfaces;

public interface IPricingService
{
    decimal CalculateFinalPrice(Entities.Game game, IEnumerable<Promotion> activePromotions);
}
