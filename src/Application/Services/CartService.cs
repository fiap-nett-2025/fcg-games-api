using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;

namespace Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repository;
    private readonly IGameRepository _gameRepository;
    private readonly IPromotionRepository _promotionRepository;
    private readonly IUserService _userService;
    private readonly IPaymentService _paymentService;
    private readonly IPricingService _pricingService;

    public CartService(
        ICartRepository repository, 
        IGameRepository gameRepository, 
        IPromotionRepository promotionRepository, 
        IUserService userService, 
        IPaymentService paymentService,
        IPricingService pricingService
        )
    {
        _repository = repository;
        _gameRepository = gameRepository;
        _promotionRepository = promotionRepository;
        _userService = userService;
        _paymentService = paymentService;
        _pricingService = pricingService;
    }

    public async Task<CartDTO> CreateCartAsync(string userId)
    {
        if (!await _userService.UserExistAsync(userId))
            throw new NotFoundException("Usuário não cadastrado.");

        var existintgCart = await _repository.GetByIdAsync(userId);
        if (existintgCart is not null)
            throw new BusinessErrorDetailsException("Esse usuário já possui um carrinho");
        try
        {
            var cart = new Cart(userId);
            return MapToDto(cart);
        }
        catch (ArgumentException ex)
        {
            throw new BusinessErrorDetailsException(ex.Message);
        }
    }

    public async Task DeleteCartAsync(string id)
    {
        var cart = await _repository.GetByIdAsync(id);
        if (cart is null)
            throw new NotFoundException("Carrinho não encontrado");
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<CartDTO>> GetAllCartsAsync()
    {
        var carts = await _repository.GetAllAsync();
        return carts.Select(cart => MapToDto(cart)).ToList();
    }

    public async Task<CartDTO> GetCartByIdAsync(string id)
    {
        var cart = await _repository.GetByIdAsync(id);
        if (cart is null)
            throw new NotFoundException("Carrinho não encontrado");
        return MapToDto(cart);
    }

    public async Task AddAGameToCartAsync(string id, int gameId)
    {
        var cart = await _repository.GetByIdAsync(id);
        if (cart is null)
            throw new NotFoundException("Carrinho não encontrado");

        cart.AddGame(gameId);
        await _repository.UpdateAsync(cart);
    }

    public async Task RemoveAGameToCartAsync(string id, int gameId)
    {
        var cart = await _repository.GetByIdAsync(id);
        if (cart is null)
            throw new NotFoundException("Carrinho não encontrado");

        cart.RemoveGame(gameId);
        await _repository.UpdateAsync(cart);
    }

    public async Task ClearCartAsync(string id)
    {
        var cart = await _repository.GetByIdAsync(id);
        if (cart is null)
            throw new NotFoundException("Carrinho não encontrado");

        cart.ClearCart();
        await _repository.UpdateAsync(cart);
    }

    public async Task<PaymentResult> CheckoutAsync(string userId)
    {
        var cart = await _repository.GetByIdAsync(userId);
        if (cart == null)
            throw new NotFoundException("Carrinho não encontrado");

        if (!cart.GameIds.Any())
            throw new BusinessErrorDetailsException("Carrinho vazio");

        var activePromotions = await _promotionRepository.GetActivePromotionsAsync(DateTime.UtcNow);
        var gamesInCart = new List<Game>();
        foreach (int gameId in cart.GameIds)
        {
            var game = await _gameRepository.GetBy(g => g.Id == gameId);
            gamesInCart.Add(game!);
        }
        decimal totalAmount = 0;
        foreach (Game game in gamesInCart)
        {
            totalAmount += _pricingService.CalculateFinalPrice(game, activePromotions);
        }

        // Processar pagamento
        var paymentSuccess = await _paymentService.ProcessPaymentAsync(userId, cart.GameIds, totalAmount);

        if (paymentSuccess)
        {
            // Limpar carrinho após compra bem-sucedida
            cart.ClearCart();
            await _repository.UpdateAsync(cart);

            return new PaymentResult
            {
                Success = true,
                Message = "Pagamento processado com sucesso",
                TotalAmount = totalAmount
            };
        }
        else
        {
            return new PaymentResult
            {
                Success = false,
                Message = "Falha no processamento do pagamento",
                TotalAmount = totalAmount
            };
        }
    }
    private static CartDTO MapToDto(Cart cart)
    {
        return new CartDTO
        {
            UserId = cart.UserId,
            GameIds = cart.GameIds
        };
    }
}
