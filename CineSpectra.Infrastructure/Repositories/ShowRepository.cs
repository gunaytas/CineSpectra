using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using CineSpectra.Domain.Enums;
using CineSpectra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CineSpectra.Infrastructure.Repositories
{
    public class ShowRepository : GenericRepository<Show>, IShowRepository
    {

        public ShowRepository(CineSpectraDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Show>> GetPopularShowsAsync(int count)
        {
            return await _context.Shows
                .Include(s => s.Genres) 
                .OrderByDescending(s => s.CriteriaAverageScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Show>> GetShowsByGenreAsync(int genreId)
        {
            return await _context.Shows
                .Include(s => s.Genres) 
                .Where(s => s.Genres.Any(g => g.Id == genreId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Show>> SearchShowsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Show>();

            return await _context.Shows
                .Include(s => s.Genres) 
                .Where(s => EF.Functions.Like(s.Title, $"%{searchTerm}%"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Show>> GetAllMoviesAsync()
        {
            return await _context.Shows
                .Include(s => s.Genres)
                .Where(s => s.Type == MediaType.Movie)
                .ToListAsync();
        }

        public async Task<IEnumerable<Show>> GetAllTvShowsAsync()
        {
            return await _context.Shows
                .Include(s => s.Genres)
                .Where(s => s.Type == MediaType.TVShow)
                .ToListAsync();
        }

        public async Task<Show?> GetShowDetailWithCastAndGenresAsync(int id)
        {
            return await _context.Shows
                .Include(s => s.Genres)
                .Include(s => s.Seasons)
                    .ThenInclude(se => se.Episodes)
                .Include(s => s.Actors) 
                .Include(s => s.Characters)
                    .ThenInclude(c => c.Actor)
                .FirstOrDefaultAsync(s => s.Id == id);

        }

    }
}