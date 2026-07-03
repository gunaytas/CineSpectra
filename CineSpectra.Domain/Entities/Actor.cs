using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CineSpectra.Domain.Common;

namespace CineSpectra.Domain.Entities
{
    public class Actor: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public DateTime? BirthDate { get; set; }
        public double AverageScore { get; set; } = 0.0;
        public string? ProfileImageUrl { get; set; }

        public ICollection<Character> Characters { get; set; } = new List<Character>();
        public ICollection<ActorRating> Ratings { get; set; } = new List<ActorRating>();
        public ICollection<Show> Shows { get; set; } = new HashSet<Show>();
    }
}
