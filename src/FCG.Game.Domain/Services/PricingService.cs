using FCG.Game.Domain.Entities;
using FCG.Game.Domain.Interfaces;

namespace FCG.Game.Domain.Services;

public class PricingService : IPricingService
{
    public decimal CalculateFinalPrice(Entities.Game game, IEnumerable<Promotion> activePromotions)
    {
        var applicablePromotions = activePromotions
            .Where(p => p.IsActive() && game.Genre.Contains(p.TargetGenre));

        if (!applicablePromotions.Any())
        {
            return game.Price;
        }
        var minimunFinalPrice = applicablePromotions
            .Select(p => p.CalculateDiscountedPrice(game))
            .Min();
        return minimunFinalPrice;
    }
}
