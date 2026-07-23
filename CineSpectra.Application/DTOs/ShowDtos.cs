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
        public int Type { get; set; } 
        public DateTime? ReleaseDate { get; set; }
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
        public int Type { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public List<GenreDto> Genres { get; set; } = new();
        public List<SeasonDto> Seasons { get; set; } = new();
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
    }
}