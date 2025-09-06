using Application.DTOs;
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
            throw new Exception($"Promoção com o nome '{model.Name}' já existe.");

        var promotion = new Promotion
        (
            model.Name,
            model.Description,
            model.DiscountPercentage,
            model.TargetGenre,
            model.StartDate,
            model.EndDate
        );

        await _promotionRepository.AddAsync(promotion);

        return new PromotionDTO
        {
            Id = (int)promotion.Id,
            Name = promotion.Name ?? "",
            Description = promotion.Description ?? "",
            DiscountPercentage = promotion.DiscountPercentage,
            TargetGenre = promotion.TargetGenre,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate
        };
    }

    public async Task DeletePromotionAsync(int id)
    {
        var promotion = await _promotionRepository.GetBy(g => g.Id.Equals(id));

        if (promotion is null)
            throw new Exception($"Promoção {id} não encontrada.");

        await _promotionRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PromotionDTO>> GetAllPromotionsAsync()
    {
        var promotions = await _promotionRepository.GetAllAsync();

        var promotionDTO = promotions.Select(promo => new PromotionDTO
        {
            Id = (int)promo.Id,
            Name = promo.Name ?? "",
            Description = promo.Description ?? "",
            DiscountPercentage = (int)promo.DiscountPercentage,
            TargetGenre = promo.TargetGenre,
            StartDate = promo.StartDate,
            EndDate = promo.EndDate,
        }).ToList();
        return promotionDTO;
    }

    public async Task<PromotionDTO> GetPromotionByIdAsync(int id)
    {
        var promotion = await _promotionRepository.GetBy(promo => promo.Id.Equals(id));
        if (promotion is null)
            throw new Exception("Promoção inexistente.");

        return new PromotionDTO
        {
            Id = (int)promotion.Id,
            Name = promotion.Name ?? "",
            Description = promotion.Description ?? "",
            DiscountPercentage = (int)promotion.DiscountPercentage,
            TargetGenre = promotion.TargetGenre,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
        };
    }

    public async Task UpdatePromotionAsync(int id, UpdatePromotionDTO model)
    {
        var promotion = await _promotionRepository.GetBy(g => g.Id.Equals(id));
        var ExistingPromotion = await _promotionRepository.GetBy(g => g.Name.ToUpper() == model.Name.ToUpper() && g.Id != id);

        if (ExistingPromotion is not null)
            throw new Exception($"Promoção com o nome '{model.Name}' já existe.");

        if (promotion is null)
            throw new Exception($"Promoção {id} não encontrada.");

        if (!string.IsNullOrWhiteSpace(model.Name))
            promotion.Name = model.Name;

        if (!string.IsNullOrWhiteSpace(model.Description))
            promotion.Description = model.Description;

        if (model.DiscountPercentage.HasValue)
            promotion.DiscountPercentage = model.DiscountPercentage.Value;

        if (model.TargetGenre.HasValue)
            promotion.TargetGenre = model.TargetGenre.Value;

        //validations
        Promotion.ValidateName(promotion.Name);
        Promotion.ValidateDescription(promotion.Description);
        Promotion.ValidateDiscountPercentage(promotion.DiscountPercentage);
        Promotion.ValidateStartDate(promotion.StartDate);
        Promotion.ValidateEndDate(promotion.EndDate, promotion.StartDate);

        await _promotionRepository.UpdateAsync(promotion);
    }
}
