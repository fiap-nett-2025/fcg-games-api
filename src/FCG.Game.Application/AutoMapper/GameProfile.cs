using AutoMapper;
using FCG.Game.Application.DTOs;

namespace FCG.Game.Application.AutoMapper
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<CreateGameDTO, Domain.Entities.Game>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();
        }
    }
}
