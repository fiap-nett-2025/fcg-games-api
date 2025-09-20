using Domain.Entities;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository;

public class CartRepository(GameDbContext context) : ICartRepository
{
    private readonly GameDbContext _context = context;
    public async Task AddAsync(Cart cart)
    {
        await _context.AddAsync(cart);
        await _context.SaveChangesAsync();
    }

    public async Task<Cart?> GetByIdAsync(string userId)
    {
        return await _context.Carts.FindAsync(userId);
    }
    public async Task DeleteAsync(string userId)
    {
        var cart = await GetByIdAsync(userId);
        if (cart is not null)
        {
            _context.Carts.Attach(cart);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Cart>> GetAllAsync()
    {
        return await _context.Carts
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateAsync(Cart cart)
    {
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync();
    }
}
