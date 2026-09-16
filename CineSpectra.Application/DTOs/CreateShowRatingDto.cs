using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace CineSpectra.Application.DTOs
{
    public class CreateShowRatingDto
    {
        public int ShowId { get; set; }
        public string? UserId { get; set; }
        public string? Comment { get; set; }
        public string UserIpAddress { get; set; } = "127.0.0.1";

        public double OverallScore { get; set; }

        public List<RatingValueDto> CriteriaRatings { get; set; } = new();
    }

    public class RatingValueDto
    {
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }
}