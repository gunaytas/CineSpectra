using CineSpectra.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Application.DTOs
{
    public class ShowRatingStatsDto
    {
        public int ShowId { get; set; }
        public double OverallAverageScore { get; set; } 
        public int TotalVotes { get; set; } 
        public List<CriteriaAverageDto> CriteriaAverages { get; set; } = new();
    }

    public class CriteriaAverageDto
    {
        public int CriteriaId { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public double AverageScore { get; set; } 
        public CriteriaTarget Target { get; set; }
    }
}