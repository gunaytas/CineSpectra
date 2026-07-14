using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Domain.Common;

namespace CineSpectra.Domain.Entities
{
    public class MediaRatingSubValue : BaseEntity
    {
        public int MediaRatingId { get; set; }
        public MediaRating MediaRating { get; set; } = null!;

        public int CriteriaId { get; set; }
        public RatingCriteria Criteria { get; set; } = null!;

        public int Score { get; set; }
    }
}