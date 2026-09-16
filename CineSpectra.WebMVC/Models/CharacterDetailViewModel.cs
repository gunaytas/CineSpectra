using CineSpectra.Application.DTOs;
using System.Collections.Generic;

namespace CineSpectra.WebMVC.Models
{
    public class CharacterDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public double AverageScore { get; set; }

        public int? ActorId { get; set; }
        public string? ActorName { get; set; }

        public List<CharacterShowCreditViewModel> Shows { get; set; } = new();
        public ShowRatingStatsDto? RatingStats { get; set; }
    }

    public class CharacterShowCreditViewModel
    {
        public int ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? ShowPosterUrl { get; set; }
        public int? ShowYear { get; set; }
        public double AverageScore { get; set; }
    }

    public class SubmitCharacterRatingViewModel
    {
        public int CharacterId { get; set; }
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }
}