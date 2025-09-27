using Application.DTOs.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.AutoMapper
{
    public class DomainToDtoGameES : Profile
    {
        public DomainToDtoGameES() { 
            CreateMap<Game, SearchGameResponseDTO>();
        }
    }
}
