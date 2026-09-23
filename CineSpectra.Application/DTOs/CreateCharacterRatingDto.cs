using System.Collections.Generic;

namespace CineSpectra.Application.DTOs
{
    public class CreateCharacterRatingDto
    {
        public int CharacterId { get; set; }
        public string? UserId { get; set; }
        public double OverallScore { get; set; }
        public bool IsDetailed { get; set; }
        public string? Comment { get; set; }
        public List<CharacterSubRatingDto> SubRatings { get; set; } = new();
    }

    public class CharacterSubRatingDto
    {
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }
}