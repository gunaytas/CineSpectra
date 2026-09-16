using CineSpectra.Domain.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CineSpectra.WebMVC.Models
{
    public class ShowRateViewModel
    {
        public int ShowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public int? ReleaseYear { get; set; }
        public double CurrentAverageScore { get; set; }
        public MediaType Type { get; set; } 

        public List<CriteriaVoteItemViewModel> Criterias { get; set; } = new();
        public List<SeasonRateItemDto> Seasons { get; set; } = new();

    }

    public class CriteriaVoteItemViewModel
    {
        public int CriteriaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
        public int SelectedScore { get; set; } = 7; // Varsayılan puan
    }

    public class SubmitFullRatingInputModel
    {
        public int ShowId { get; set; }
        public double OverallScore { get; set; }
        public string? Comment { get; set; }
        public bool IsDetailed { get; set; }

        [JsonPropertyName("subRatings")]
        public List<SubCriteriaRatingInputModel> SubRatings { get; set; } = new();
        public List<SubCriteriaRatingInputModel> CriteriaRatings
        {
            get => SubRatings;
            set => SubRatings = value;
        }
    }

    public class SubCriteriaRatingInputModel
    {
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }

    public class SubmitRatingViewModel
    {
        public int ShowId { get; set; }
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }

    public class SeasonRateItemDto
    {
        public int SeasonId { get; set; }
        public int SeasonNumber { get; set; }
        public string SeasonName { get; set; } = string.Empty;
        public double CurrentAverageScore { get; set; }
        public List<EpisodeRateItemDto> Episodes { get; set; } = new();
    }

    public class EpisodeRateItemDto
    {
        public int EpisodeId { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public double CurrentAverageScore { get; set; }
    }
}