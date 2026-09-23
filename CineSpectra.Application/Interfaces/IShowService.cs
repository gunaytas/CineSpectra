using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Application.DTOs;

namespace CineSpectra.Application.Interfaces
{
    public interface IShowService
    {
        Task<IEnumerable<ShowListDto>> GetPopularShowsAsync(int count);
        Task<IEnumerable<ShowListDto>> GetMoviesAsync();
        Task<IEnumerable<ShowListDto>> GetTvShowsAsync();
        Task<IEnumerable<ShowListDto>> SearchAsync(string searchTerm);

        Task<ShowDetailDto?> GetShowDetailAsync(int id, string? userId = null);
    }
}