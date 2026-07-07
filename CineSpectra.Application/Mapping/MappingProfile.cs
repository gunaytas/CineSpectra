using AutoMapper;
using CineSpectra.Application.DTOs;
using CineSpectra.Domain.Entities;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CineSpectra.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Show, ShowListDto>();
            CreateMap<Genre, GenreDto>();
            CreateMap<Season, SeasonDto>();
            CreateMap<Episode, EpisodeDto>();

            CreateMap<Show, ShowDetailDto>();
        }
    }
}