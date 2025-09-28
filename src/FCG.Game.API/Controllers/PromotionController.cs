using FCG.Game.Application.DTOs;
using FCG.Game.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Game.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PromotionController(IPromotionService promotionService) : ApiBaseController
{
    [HttpGet]
    [Authorize(Policy = "Authenticated")]
    public async Task<IActionResult> GetPromotions()
    {
        var promotions = await promotionService.GetAllPromotionsAsync();

        return Success(promotions, "Lista de promoções retornada com sucesso.");
    }

    [HttpGet("Actived")]
    public async Task<IActionResult> GetActivedPromotions()
    {
        var promotions = await promotionService.GetAllActivedPromotionsAsync();
        return Success(promotions, "Lista de promoções ativas retornada com sucesso.");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPromotion(int id)
    {
        var promotion = await promotionService.GetPromotionByIdAsync(id);
        return Success(promotion, "Promoção encontrada.");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePromotion(CreatePromotionDTO model)
    {
        var promotion = await promotionService.CreatePromotionAsync(model);
        return CreatedResponse(promotion, "Promoção criada com sucesso.");
    }


    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PartialUpdateGame(int id, UpdatePromotionDTO model)
    {
        await promotionService.UpdatePromotionAsync(id, model);
        return Success("Promoção atualizada com sucesso!");
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePromotion([FromRoute] int id)
    {

        await promotionService.DeletePromotionAsync(id);
        return NoContent();
    }
}
