using CineSpectra.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Application.DTOs
{
    public class ShowListDto
    { 
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public double AverageScore { get; set; }
        public MediaType Type { get; set; } 
        public DateTime? ReleaseDate { get; set; }
        public double? EpisodeAudienceScore { get; set; }
        public List<GenreDto> Genres { get; set; } = new();
    }

    public class ShowDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public string? Director { get; set; }
        public string? ProductionCompany { get; set; }
        public string? Writers { get; set; } 
        public double AverageScore { get; set; }
        public double? EpisodeAudienceScore { get; set; }
        public MediaType Type { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public List<GenreDto> Genres { get; set; } = new();
        public List<SeasonDto> Seasons { get; set; } = new();
        public List<ActorDto> Actors { get; set; } = new();
        public List<CharacterDto> Characters { get; set; } = new();

        public ShowRatingStatsDto? RatingStats { get; set; }
        public List<ShowCommentDto> Comments { get; set; } = new();
    }

    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ActorId { get; set; }
        public string ActorName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public double AverageScore { get; set; }
               
    }

    public class ActorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }
        public double AverageScore { get; set; }
    }

    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class SeasonDto
    {
        public int Id { get; set; }
        public int SeasonNumber { get; set; }
        public string? Name { get; set; }
        public string? Overview { get; set; }
        public string? PosterPath { get; set; }
        public double AverageScore { get; set; }
        public List<EpisodeDto> Episodes { get; set; } = new();
    }

    public class EpisodeDto
    {
        public int Id { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public string? Overview { get; set; }
        public DateTime? AirDate { get; set; }
        public double? UserScore { get; set; }
    }

    public class ShowCommentDto
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public double Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}