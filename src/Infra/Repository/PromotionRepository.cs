using Domain.Entities;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infra.Repository;

public class PromotionRepository(GameDbContext context) : IPromotionRepository
{
    private GameDbContext _context = context;
    public async Task AddAsync(Promotion promotion)
    {
        await _context.AddAsync(promotion);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var promotion = await GetBy(g => g.Id.Equals(id));
        if (promotion != null)
        {
            _context.Promotions.Attach(promotion);
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Promotion>> GetAllAsync()
    {
        return await _context.Promotions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Promotion?> GetBy(Expression<Func<Promotion, bool>> condition)
    {
        return await _context.Promotions
            .AsNoTracking()
            .FirstOrDefaultAsync(condition);
    }

    public Task UpdateAsync(Promotion promotion)
    {
        _context.Promotions.Update(promotion);
        return _context.SaveChangesAsync();
    }
}
