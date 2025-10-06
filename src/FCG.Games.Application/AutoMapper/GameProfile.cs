using AutoMapper;
using FCG.Games.Application.DTOs;
using FCG.Games.Domain.Entities;

namespace FCG.Games.Application.AutoMapper
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<CreateGameDTO, Game>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ReverseMap();
        }
    }
}
