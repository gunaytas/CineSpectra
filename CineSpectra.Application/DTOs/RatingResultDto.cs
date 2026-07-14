using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Application.DTOs
{
    public class RatingResultDto
    {
        public bool IsSuccess { get; set; }
        public int ShowId { get; set; }
        public double CalculatedWeightedScore { get; set; } 
        public double NewAverageScoreOfShow { get; set; }  
        public string Message { get; set; } = string.Empty;

    }
}