using Domain.Entities;

namespace Domain.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<Cart>> GetAllAsync();
    Task<Cart?> GetByIdAsync(string userId);
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
    Task DeleteAsync(string userId);
}
