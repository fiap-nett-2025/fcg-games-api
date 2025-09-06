using Application.DTOs;

namespace Application.Interfaces;

public interface IPromotionService
{
    Task<IEnumerable<PromotionDTO>> GetAllPromotionsAsync();
    Task<PromotionDTO> GetPromotionByIdAsync(int id);
    Task<PromotionDTO> CreatePromotionAsync(CreatePromotionDTO model);
    Task UpdatePromotionAsync(int id, UpdatePromotionDTO model);
    Task DeletePromotionAsync(int id);
}
