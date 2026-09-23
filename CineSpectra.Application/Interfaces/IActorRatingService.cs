using CineSpectra.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Application.Interfaces
{
    public interface IActorRatingService
    {
        Task<RatingResultDto> SubmitActorRatingAsync(CreateActorRatingDto dto);
    }
}
