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
        public async Task<MediaRating?> GetUserShowRatingAsync(int showId, string? userId)
        {
            return await _context.MediaRatings
                .Include(r => r.SubValues)
                .FirstOrDefaultAsync(r => r.ShowId == showId &&
                                          r.SeasonId == null &&
                                          r.EpisodeId == null &&
                                          r.UserId == userId);
        }

        // Kural 1: Kullanıcı bu sezonun herhangi bir bölümüne oy vermiş mi?
        public async Task<bool> HasUserRatedAnyEpisodeInSeasonAsync(int seasonId, string userId)
        {
            return await _context.MediaRatings
                .AnyAsync(r => r.SeasonId == seasonId && r.EpisodeId != null && r.UserId == userId);
        }

        // Kural 2: Kullanıcı sezonun geneline oy vermiş mi?
        public async Task<bool> HasUserRatedSeasonAsync(int seasonId, string userId)
        {
            return await _context.MediaRatings
                .AnyAsync(r => r.SeasonId == seasonId && r.EpisodeId == null && r.UserId == userId);
        }

        public async Task<MediaRating?> GetUserSeasonRatingAsync(int seasonId, string? userId)
        {
            return await _context.MediaRatings
                .FirstOrDefaultAsync(r => r.SeasonId == seasonId && r.EpisodeId == null && r.UserId == userId);
        }

        public async Task<MediaRating?> GetUserEpisodeRatingAsync(int episodeId, string? userId)
        {
            return await _context.MediaRatings
                .FirstOrDefaultAsync(r => r.EpisodeId == episodeId && r.UserId == userId);
        }
    }
}