using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Domain.Common;

namespace CineSpectra.Domain.Entities
{
    public class Episode : BaseEntity
    {
        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!;

        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Overview { get; set; }
        public DateTime? AirDate { get; set; }
        public double AverageScore { get; set; } = 0.0;

    }
}