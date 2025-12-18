using FCG.Games.Domain.Entities;
using FCG.Games.Domain.Interfaces;

namespace FCG.Games.Domain.Services;

public class PricingService : IPricingService
{
    public decimal CalculateFinalPrice(Game game, IEnumerable<Promotion> activePromotions)
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
