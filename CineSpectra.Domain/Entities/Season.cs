using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Domain.Common;

namespace CineSpectra.Domain.Entities
{
    public class Season : BaseEntity
    {
        public int ShowId { get; set; }
        public Show Show { get; set; } = null!;

        public int SeasonNumber { get; set; }
        public string? Name { get; set; }
        public string? Overview { get; set; }
        public string? PosterPath { get; set; }
        public double AverageScore { get; set; } = 0.0; 

        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }
}