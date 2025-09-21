using Application.DTOs;

namespace Application.Interfaces;

public interface ICartService
{
    Task<IEnumerable<CartDTO>> GetAllCartsAsync();
    Task<CartDTO> GetCartByIdAsync(string id);
    Task<CartDTO> CreateCartAsync(string userId);
    Task AddAGameToCartAsync(string id, int gameId);
    Task RemoveAGameToCartAsync(string id, int gameId);
    Task ClearCartAsync(string id);
    Task DeleteCartAsync(string id);
    Task<PaymentResult> CheckoutAsync(string userId);
}
