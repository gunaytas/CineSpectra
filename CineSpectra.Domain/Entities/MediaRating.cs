using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Domain.Common;

namespace CineSpectra.Domain.Entities
{
    public class MediaRating : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string? Comment { get; set; }

        public double CalculatedRatingValue { get; set; }

        public int ShowId { get; set; }
        public Show Show { get; set; } = null!;

        public int? SeasonId { get; set; }
        public Season? Season { get; set; }

        public int? EpisodeId { get; set; }
        public Episode? Episode { get; set; }

        public ICollection<MediaRatingSubValue> SubValues { get; set; } = new List<MediaRatingSubValue>();
    }
}