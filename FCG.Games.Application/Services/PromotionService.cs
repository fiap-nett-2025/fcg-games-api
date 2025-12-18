using FCG.Games.Application.Exceptions;
using FCG.Games.Domain.Entities;
using FCG.Games.Application.DTOs;
using FCG.Games.Application.Interfaces;
using FCG.Games.Domain.Interfaces.Repository;

namespace FCG.Games.Application.Services;

public class PromotionService(IPromotionRepository promotionRepository) : IPromotionService
{
    public async Task<PromotionDTO> CreatePromotionAsync(CreatePromotionDTO model)
    {
        var existingPromotion = await promotionRepository.GetBy(g => g.Name.ToUpper() == model.Name.ToUpper());
        if (existingPromotion is not null)
            throw new BusinessErrorDetailsException($"Promoção com o nome '{model.Name}' já existe.");

        try
        {
            var promotion = new Promotion(
                model.Name,
                model.Description,
                model.DiscountPercentage,
                model.TargetGenre,
                model.StartDate,
                model.EndDate
            );
            await promotionRepository.AddAsync(promotion);
            return MapToDto(promotion);
        }
        catch (ArgumentException ex)
        {
            throw new BusinessErrorDetailsException(ex.Message);
        }
    }

    public async Task DeletePromotionAsync(int id)
    {
        var promotion = await promotionRepository.GetBy(g => g.Id.Equals(id));

        if (promotion is null)
            throw new NotFoundException($"Promoção {id} não encontrada.");

        await promotionRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PromotionDTO>> GetAllPromotionsAsync()
    {
        var promotions = await promotionRepository.GetAllAsync();
        return promotions.Select(promo => MapToDto(promo)).ToList(); ;
    }

    public async Task<IEnumerable<PromotionDTO>> GetAllActivedPromotionsAsync()
    {
        var currentDate = DateTime.UtcNow;
        var promotions = await promotionRepository.GetActivePromotionsAsync(currentDate);
        return promotions.Select(promo => MapToDto(promo)).ToList();
    }

    public async Task<PromotionDTO> GetPromotionByIdAsync(int id)
    {
        var promotion = await promotionRepository.GetBy(promo => promo.Id.Equals(id));
        if (promotion is null)
            throw new NotFoundException("Promoção inexistente.");

        return MapToDto(promotion);
    }

    public async Task UpdatePromotionAsync(int id, UpdatePromotionDTO model)
    {
        var promotion = await promotionRepository.GetBy(g => g.Id.Equals(id));
        if (promotion is null)
            throw new NotFoundException($"Promoção {id} não encontrada.");
        
        var ExistingPromotion = await promotionRepository.GetBy(g => g.Name.ToUpper() == model.Name.ToUpper() && g.Id != id);
        if (ExistingPromotion is not null)
            throw new BusinessErrorDetailsException($"Promoção com o nome '{model.Name}' já existe.");


        if (!string.IsNullOrWhiteSpace(model.Name))
            promotion.UpdateName(model.Name);

        if (!string.IsNullOrWhiteSpace(model.Description))
            promotion.UpdateDescription(model.Description);

        if (model.DiscountPercentage.HasValue)
            promotion.UpdateDiscountPercentage(model.DiscountPercentage.Value);

        if (model.TargetGenre.HasValue)
            promotion.UpdateTargetGenre(model.TargetGenre.Value);

        await promotionRepository.UpdateAsync(promotion);
    }

    private static PromotionDTO MapToDto(Promotion promotion) => new PromotionDTO
    {
        Id = promotion.Id,
        Name = promotion.Name,
        Description = promotion.Description,
        DiscountPercentage = promotion.DiscountPercentage,
        TargetGenre = promotion.TargetGenre,
        StartDate = promotion.StartDate,
        EndDate = promotion.EndDate
    };
}
