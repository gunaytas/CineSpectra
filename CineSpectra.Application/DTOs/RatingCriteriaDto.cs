using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Domain.Enums;
using CineSpectra.Domain.Entities;


namespace CineSpectra.Application.DTOs
{
    public class RatingCriteriaDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
        public CriteriaTarget Target { get; set; } 
    }
}