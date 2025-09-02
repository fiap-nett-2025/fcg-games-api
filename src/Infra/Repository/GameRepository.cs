using Domain.Entities;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infra.Repository;

public class GameRepository(GameDbContext context) : IGameRepository
{
    private readonly GameDbContext _context = context;
    public async Task AddAsync(Game game)
    {
        await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var game = await GetBy(g => g.Id.Equals(id));
        if (game != null)
        {
            _context.Games.Attach(game);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Game?> GetBy(Expression<Func<Game, bool>> condition)
    {
        return await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(condition);
    }

    public Task UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        return _context.SaveChangesAsync();
    }
}
