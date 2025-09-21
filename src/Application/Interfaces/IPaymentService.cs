using Domain.Entities;

namespace Application.Interfaces;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(string userId, List<int> gameIds, decimal totalAmount);
}
