using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ApiBaseController
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetCarts()
    {
        var carts = await _cartService.GetAllCartsAsync();
        return Success(carts, "Lista de carrinhos retornada com sucesso.");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCart(string id)
    {
        var cart = await _cartService.GetCartByIdAsync(id);
        return Success(cart, "Carrinho encontrado.");
    }

    [HttpPost("{userId}/checkout")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> Checkout(string userId)
    {
        try
        {
            var result = await _cartService.CheckoutAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{userId}/games/{gameId}")]
    public async Task<IActionResult> AddGame(string userId, int gameId)
    {
        try
        {
            await _cartService.AddAGameToCartAsync(userId, gameId);
            return Ok(new {message = "Jogo adicionado ao carrinho"});
        }
        catch (Exception ex)
        {

            return BadRequest(new {error = ex.Message});
        }
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> CreateCart(string userId)
    {
        try
        {
            var cart = await _cartService.CreateCartAsync(userId);
            return CreatedAtAction(nameof(GetCart), new { id = cart.UserId }, cart);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{userId}")]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> ClearCart(string userId)
    {
        try
        {
            await _cartService.ClearCartAsync(userId);
            return Ok(new { message = "Carrinho limpo" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{userId}/games/{gameId}")]
    public async Task<IActionResult> RemoveGame(string userId, int gameId)
    {
        try
        {
            await _cartService.RemoveAGameToCartAsync(userId, gameId);
            return Ok(new { message = "Jogo removido do carrinho." });
        }
        catch (Exception)
        {

            return BadRequest(new {message = "Jogo não está no carrinho."});
        }
    }
}
