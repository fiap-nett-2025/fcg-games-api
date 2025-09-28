using FCG.Game.Domain.Entities;
using FCG.Game.Domain.Interfaces;
using FCG.Game.Infra.Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCG.Game.Infra.Data.Repository;

public class PromotionRepository(FcgGameDbContext context) : IPromotionRepository
{
    public async Task AddAsync(Promotion promotion)
    {
        await context.AddAsync(promotion);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var promotion = await GetBy(g => g.Id.Equals(id));
        if (promotion != null)
        {
            context.Promotions.Attach(promotion);
            context.Promotions.Remove(promotion);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync(DateTime currentDate)
    {
        return await context.Promotions
            .AsNoTracking()
            .Where(p => p.StartDate <= currentDate && p.EndDate >= currentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Promotion>> GetAllAsync()
    {
        return await context.Promotions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Promotion?> GetBy(Expression<Func<Promotion, bool>> condition)
    {
        return await context.Promotions
            .AsNoTracking()
            .FirstOrDefaultAsync(condition);
    }

    public Task UpdateAsync(Promotion promotion)
    {
        context.Promotions.Update(promotion);
        return context.SaveChangesAsync();
    }
}
