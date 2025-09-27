using Application.DTOs.Requests;
using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Application.AutoMapper
{
    public class GameToSearchDocProfile : Profile
    {
        public GameToSearchDocProfile()
        {
            CreateMap<Game, GameSearchDoc>();
            CreateMap<CreateGameDTO, Game>(); // você já tem
        }
    }
}
