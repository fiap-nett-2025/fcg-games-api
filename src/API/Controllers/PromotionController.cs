using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PromotionController : ApiBaseController
{
    private readonly IPromotionService _promotionService;

    public PromotionController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetPromotions()
    {
        var promotions = await _promotionService.GetAllPromotionsAsync();

        return Success(promotions, "Lista de promoções retornada com sucesso.");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPromotion(int id)
    {
        var promotion = await _promotionService.GetPromotionByIdAsync(id);

        return Success(promotion, "Promoção encontrada.");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePromotion(CreatePromotionDTO model)
    {
        var promotion = await _promotionService.CreatePromotionAsync(model);
        return CreatedResponse(promotion, "Promoção criada com sucesso.");
    }


    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PartialUpdateGame(int id, UpdatePromotionDTO model)
    {
        await _promotionService.UpdatePromotionAsync(id, model);
        return Success("Promoção atualizada com sucesso!");
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePromotion([FromRoute] int id)
    {

        await _promotionService.DeletePromotionAsync(id);
        return NoContent();
    }
}
