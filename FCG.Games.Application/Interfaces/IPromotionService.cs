using FCG.Games.Application.DTOs;

namespace FCG.Games.Application.Interfaces;

public interface IPromotionService
{
    Task<IEnumerable<PromotionDTO>> GetAllPromotionsAsync();
    Task<IEnumerable<PromotionDTO>> GetAllActivedPromotionsAsync();
    Task<PromotionDTO> GetPromotionByIdAsync(int id);
    Task<PromotionDTO> CreatePromotionAsync(CreatePromotionDTO model);
    Task UpdatePromotionAsync(int id, UpdatePromotionDTO model);
    Task DeletePromotionAsync(int id);
}
