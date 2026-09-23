using System.Collections.Generic;

namespace CineSpectra.Application.DTOs
{
    public class CreateActorRatingDto
    {
        public int ActorId { get; set; }
        public string? UserId { get; set; }
        public double OverallScore { get; set; }
        public bool IsDetailed { get; set; }
        public string? Comment { get; set; }
        public List<ActorSubRatingDto> SubRatings { get; set; } = new();
    }

    public class ActorSubRatingDto
    {
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }
}