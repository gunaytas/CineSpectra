using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Domain.Entities;
using System.Threading.Tasks;

namespace CineSpectra.Application.Interfaces
{
    public interface IShowRatingRepository : IGenericRepository<MediaRating>
    {
        Task<List<MediaRating>> GetRatingsByShowIdWithCriteriaAsync(int showId);

    }
}