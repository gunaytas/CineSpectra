using CineSpectra.Application.DTOs;
using CineSpectra.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Formatters;
using MediaType = CineSpectra.Domain.Enums.MediaType;
namespace CineSpectra.WebMVC.Models;

public class ShowDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Director { get; set; } 
    public string? ProductionCompany { get; set; } 
    public string? Writers { get; set; } 
    public DateTime? ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public double AverageScore { get; set; }
    public double? EpisodeAudienceScore { get; set; }
    public MediaType Type { get; set; }
    public int? SeasonCount { get; set; }
    public int? EpisodesCount { get; set; }

    public ShowRatingStatsDto RatingStats { get; set; } = new();
    public List<CineSpectra.Application.DTOs.GenreDto> Genres { get; set; } = new();
    public List<CineSpectra.Application.DTOs.CharacterDto> Characters { get; set; } = new();
    public List<CineSpectra.Application.DTOs.ActorDto> Actors { get; set; } = new();
    public List<CineSpectra.Application.DTOs.SeasonDto> Seasons { get; set; } = new();
    public List<EpisodesDto> Episodes { get; set; } = new();
    public List<CineSpectra.Application.DTOs.RatingCriteriaDto> ActiveCriteria { get; set; } = new();
}


public class RatingCriteriaDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Weight { get; set; }
    public CriteriaTarget Target { get; set; } 
}

public class GenreDto
{
    public string Name { get; set; } = string.Empty;
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

public class SeasonsDto
{
    public int Id { get; set; }
    public int SeasonNumber { get; set; }
    public string? Name { get; set; }
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public double AverageScore { get; set; }
    public List<EpisodesDto> Episodes { get; set; } = new();
}

public class EpisodesDto
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int EpisodeNumber { get; set; }
    public string? Title { get; set; }
    public string? Overview { get; set; }
    public DateTime? AirDate { get; set; }
    public double AverageScore { get; set; }
}