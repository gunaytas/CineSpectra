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

            CreateMap<Actor, ActorDto>();

            CreateMap<Character, CharacterDto>()
                .ForMember(dest => dest.ActorId, opt => opt.MapFrom(src => src.ActorId))
                .ForMember(dest => dest.ActorName, opt => opt.MapFrom(src => src.Actor != null ? src.Actor.Name : string.Empty));

            CreateMap<Show, ShowDetailDto>()
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => src.CriteriaAverageScore))

            .ForMember(dest => dest.Actors, opt => opt.MapFrom(src =>
                src.Characters != null
                    ? src.Characters
                        .Where(c => c.Actor != null)
                        .Select(c => c.Actor!)
                        .GroupBy(a => a.Id) 
                        .Select(g => g.First())
                        .ToList()
                    : new List<Actor>()))

            .ForMember(dest => dest.Characters, opt => opt.MapFrom(src => src.Characters));

            CreateMap<Show, ShowListDto>()
            .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => src.CriteriaAverageScore))
            .ForMember(dest => dest.EpisodeAudienceScore, opt => opt.MapFrom(src => src.EpisodeAudienceScore));

        }
    }
}