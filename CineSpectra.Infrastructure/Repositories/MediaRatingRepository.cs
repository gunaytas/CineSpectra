using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using CineSpectra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Infrastructure.Repositories
{
    public class MediaRatingRepository : GenericRepository<MediaRating>, IShowRatingRepository
    {
        public MediaRatingRepository(CineSpectraDbContext context) : base(context)
        {
        }

        public async Task<List<MediaRating>> GetRatingsByShowIdWithCriteriaAsync(int showId)
        {
            return await _context.MediaRatings
                .Include(r => r.SubValues)         
                    .ThenInclude(sv => sv.Criteria) 
                .Where(r => r.ShowId == showId)
                .ToListAsync();
        }
    }
}