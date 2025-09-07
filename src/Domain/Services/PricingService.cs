using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Services;

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
