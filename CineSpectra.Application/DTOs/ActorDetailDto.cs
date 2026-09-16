using System;
using System.Collections.Generic;

namespace CineSpectra.Application.DTOs
{
    public class ActorDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime? BirthDate { get; set; }
        public double AverageScore { get; set; }

        public ShowRatingStatsDto? RatingStats { get; set; }
        public List<ActorCreditDto> Filmography { get; set; } = new();
    }

    public class ActorCreditDto
    {
        public int ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? ShowPosterUrl { get; set; }
        public int? ShowYear { get; set; }
        public int CharacterId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}