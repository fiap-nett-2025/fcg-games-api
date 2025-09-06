using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    public PromotionService(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<PromotionDTO> CreatePromotionAsync(CreatePromotionDTO model)
    {
        var existingPromotion = await _promotionRepository.GetBy(g => g.Name.ToUpper() == model.Name.ToUpper());
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
            await _promotionRepository.AddAsync(promotion);
            return MapToDto(promotion);
        }
        catch (ArgumentException ex)
        {
            throw new BusinessErrorDetailsException(ex.Message);
        }
    }

    public async Task DeletePromotionAsync(int id)
    {
        var promotion = await _promotionRepository.GetBy(g => g.Id.Equals(id));

        if (promotion is null)
            throw new NotFoundException($"Promoção {id} não encontrada.");

        await _promotionRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PromotionDTO>> GetAllPromotionsAsync()
    {
        var promotions = await _promotionRepository.GetAllAsync();
        return promotions.Select(promo => MapToDto(promo)).ToList(); ;
    }

    public async Task<PromotionDTO> GetPromotionByIdAsync(int id)
    {
        var promotion = await _promotionRepository.GetBy(promo => promo.Id.Equals(id));
        if (promotion is null)
            throw new NotFoundException("Promoção inexistente.");

        return MapToDto(promotion);
    }

    public async Task UpdatePromotionAsync(int id, UpdatePromotionDTO model)
    {
        var promotion = await _promotionRepository.GetBy(g => g.Id.Equals(id));
        if (promotion is null)
            throw new NotFoundException($"Promoção {id} não encontrada.");
        
        var ExistingPromotion = await _promotionRepository.GetBy(g => g.Name.ToUpper() == model.Name.ToUpper() && g.Id != id);
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

        await _promotionRepository.UpdateAsync(promotion);
    }

    private static PromotionDTO MapToDto(Promotion promotion) => new PromotionDTO
    {
        Id = (int)promotion.Id,
        Name = promotion.Name,
        Description = promotion.Description,
        DiscountPercentage = promotion.DiscountPercentage,
        TargetGenre = promotion.TargetGenre,
        StartDate = promotion.StartDate,
        EndDate = promotion.EndDate
    };
}
