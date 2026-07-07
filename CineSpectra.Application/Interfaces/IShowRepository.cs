using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Domain.Entities; // Kendi Show entity sınıfının yolu

namespace CineSpectra.Application.Interfaces
{
    public interface IShowRepository : IGenericRepository<Show>
    {
        Task<IEnumerable<Show>> GetPopularShowsAsync(int count);
        Task<IEnumerable<Show>> GetShowsByGenreAsync(int genreId);
        Task<Show?> GetShowDetailWithCastAndGenresAsync(int id);
        Task<IEnumerable<Show>> SearchShowsAsync(string searchTerm);

        Task<IEnumerable<Show>> GetAllMoviesAsync();
        Task<IEnumerable<Show>> GetAllTvShowsAsync();
    }
}