using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CineSpectra.Domain.Common;
using CineSpectra.Domain.Enums;

namespace CineSpectra.Domain.Entities
{
    public class Show: BaseEntity
    {
        public string Title { get; set; } = string.Empty; 
        public string? Description { get; set; }
        public string? Director { get; set; }
        public string? Writers { get; set; }
        public string? ProductionCompany { get; set; }
        public string? CoverImageUrl { get; set; }
        public MediaType Type { get; set; }

        public int? SeasonsCount { get; set; } // sadece dizi için geçerli, film için null olabilir
        public int? EpisodesCount { get; set; } // sadece dizi için geçerli, film için null olabilir

        public double AverageScore { get; set; } = 0.0;
        public DateTime? ReleaseDate { get; set; }

        public ICollection<Actor> Actors { get; set; } = new List<Actor>(); 
        public ICollection<Character> Characters { get; set; } = new List<Character>();
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Season> Seasons { get; set; } = new List<Season>();
    }
}
