using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using CineSpectra.Infrastructure.Persistence;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CineSpectra.Infrastructure.Repositories
{
    public class ShowRepository : GenericRepository<Show>, IShowRepository
    {
        private readonly IDbConnection _dbConnection;

        public ShowRepository(CineSpectraDbContext context) : base(context)
        {
            _dbConnection = _context.Database.GetDbConnection();
        }

        public async Task<IEnumerable<Show>> GetPopularShowsAsync(int count)
        {
            var query = "SELECT * FROM \"Shows\" ORDER BY \"AverageScore\" DESC LIMIT @Count";
            return await _dbConnection.QueryAsync<Show>(query, new { Count = count });
        }

        public async Task<IEnumerable<Show>> GetShowsByGenreAsync(int genreId)
        {
            var query = @"SELECT s.* FROM ""Shows"" s 
                          INNER JOIN ""ShowGenres"" sg ON s.""Id"" = sg.""ShowId"" 
                          WHERE sg.""GenreId"" = @GenreId";
            return await _dbConnection.QueryAsync<Show>(query, new { GenreId = genreId });
        }

        public async Task<IEnumerable<Show>> SearchShowsAsync(string searchTerm)
        {
            var query = "SELECT * FROM \"Shows\" WHERE \"Title\" ILIKE @Search";
            return await _dbConnection.QueryAsync<Show>(query, new { Search = $"%{searchTerm}%" });
        }

        public async Task<IEnumerable<Show>> GetAllMoviesAsync()
        {
            var query = "SELECT * FROM \"Shows\" WHERE \"Type\" = 1";
            return await _dbConnection.QueryAsync<Show>(query);
        }

        public async Task<IEnumerable<Show>> GetAllTvShowsAsync()
        {
            var query = "SELECT * FROM \"Shows\" WHERE \"Type\" = 2";
            return await _dbConnection.QueryAsync<Show>(query);
        }

        public async Task<Show?> GetShowDetailWithCastAndGenresAsync(int id)
        {
            return await _context.Shows
                .Include(s => s.Seasons)
                    .ThenInclude(se => se.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}