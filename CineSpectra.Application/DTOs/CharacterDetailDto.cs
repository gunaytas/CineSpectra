using System.Collections.Generic;

namespace CineSpectra.Application.DTOs
{
    public class CharacterDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public double AverageScore { get; set; }

        public int? ActorId { get; set; }
        public string? ActorName { get; set; }

        public List<CharacterShowCreditDto> Shows { get; set; } = new();

        public ShowRatingStatsDto? RatingStats { get; set; }
    }

    public class CharacterShowCreditDto
    {
        public int ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? ShowPosterUrl { get; set; }
        public int? ShowYear { get; set; }
        public double AverageScore { get; set; }
    }
}