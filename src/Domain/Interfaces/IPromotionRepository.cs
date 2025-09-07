using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Interfaces;

public interface IPromotionRepository
{
    Task<Promotion?> GetBy(Expression<Func<Promotion, bool>> condition);
    Task<IEnumerable<Promotion>> GetAllAsync();
    Task<IEnumerable<Promotion>> GetActivePromotionsAsync(DateTime currentDate);
    Task AddAsync(Promotion promotion);
    Task UpdateAsync(Promotion promotion);
    Task DeleteAsync(int id);
}
