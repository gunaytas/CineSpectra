using CineSpectra.Application.DTOs;

namespace CineSpectra.WebMVC.Models
{
    public class ActorDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime? BirthDate { get; set; }
        public double AverageScore { get; set; }

        public ShowRatingStatsDto? RatingStats { get; set; }

        // Oyuncunun yer aldığı yapımlar ve canlandırdığı karakterler
        public List<ActorCreditViewModel> Filmography { get; set; } = new();
    }

    public class ActorCreditViewModel
    {
        public int ShowId { get; set; }
        public string ShowTitle { get; set; } = string.Empty;
        public string? ShowPosterUrl { get; set; }
        public int? ShowYear { get; set; }
        public int CharacterId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
    public class SubmitActorRatingViewModel
    {
        public int ActorId { get; set; }
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }
}