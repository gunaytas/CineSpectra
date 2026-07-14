using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Application.DTOs;
using System.Threading.Tasks;

namespace CineSpectra.Application.Interfaces
{
    public interface IShowRatingService
    {
        Task<RatingResultDto> SubmitShowRatingAsync(CreateShowRatingDto dto);
        Task<List<RatingCriteriaDto>> GetAllCriteriaAsync();
        Task<ShowRatingStatsDto> GetShowRatingStatsAsync(int showId);
    }
}