using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.AutoMapper
{
    public class GameDtoProfile : Profile
    {
        public GameDtoProfile()
        {
            CreateMap<CreateGameDTO, Game>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
