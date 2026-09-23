using System.Collections.Generic;

namespace CineSpectra.WebMVC.Models
{
    public class CharacterRateViewModel
    {
        public int CharacterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ActorName { get; set; }
        public int? ActorId { get; set; }
        public double CurrentAverageScore { get; set; }
        public List<CharacterCriteriaVoteItemViewModel> Criterias { get; set; } = new();
    }

    public class CharacterCriteriaVoteItemViewModel
    {
        public int CriteriaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
    }

    public class SubmitCharacterRatingInputModel
    {
        public int CharacterId { get; set; }
        public double OverallScore { get; set; }
        public bool IsDetailed { get; set; }
        public string? Comment { get; set; }
        public List<CharacterSubRatingInputModel> SubRatings { get; set; } = new();
    }

    public class CharacterSubRatingInputModel
    {
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }
}