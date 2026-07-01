using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CineSpectra.Domain.Common;

namespace CineSpectra.Domain.Entities
{
    public class Character: BaseEntity
    {

        public int ActorId { get; set; }
        public Actor Actor { get; set; } = null!;

        public string Name { get; set; } = string.Empty; 
        public string? ImageUrl { get; set; }

        public double AverageScore { get; set; } = 0.0;

        public ICollection<CharacterRating> Ratings { get; set; } = new List<CharacterRating>();
        public ICollection<Show> Shows { get; set; } = new List<Show>();
    }
}
