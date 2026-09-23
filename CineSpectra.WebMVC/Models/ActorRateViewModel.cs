using System.Collections.Generic;

namespace CineSpectra.WebMVC.Models
{
    public class ActorRateViewModel
    {
        public int ActorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public int? Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public double CurrentAverageScore { get; set; }

        public List<ActorCriteriaVoteItemViewModel> Criterias { get; set; } = new();
    }

    public class ActorCriteriaVoteItemViewModel
    {
        public int CriteriaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
    }

    public class SubmitActorRatingInputModel
    {
        public int ActorId { get; set; }
        public double OverallScore { get; set; }
        public bool IsDetailed { get; set; }
        public string? Comment { get; set; }
        public List<ActorSubRatingInputModel> SubRatings { get; set; } = new();
    }

    public class ActorSubRatingInputModel
    {
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }
}