using CineSpectra.Application.DTOs;    
using CineSpectra.Application.Interfaces; 
using CineSpectra.Domain.Entities;
using CineSpectra.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CineSpectra.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IShowService _showService;

        public HomeController(IShowService showService)
        {
            _showService = showService;
        }

        public async Task<IActionResult> Index()
        {
            var popularShowsResult = await _showService.GetPopularShowsAsync(5);
            var popularList = popularShowsResult.ToList();

            var movies = await _showService.GetMoviesAsync();
            var tvShows = await _showService.GetTvShowsAsync();

            var allShows = movies.Concat(tvShows).ToList();

            var recentList = allShows
                .OrderByDescending(s => s.ReleaseDate)
                .Take(5)
                .ToList();

            // Hero Banner için detaylý yapým nesnesi 
            ShowDetailDto? heroShowDto = null;
            var topPopular = popularList.FirstOrDefault();

            if (topPopular != null)
            {
                heroShowDto = await _showService.GetShowDetailAsync(topPopular.Id);
            }
            else if (recentList.FirstOrDefault() != null)
            {
                heroShowDto = await _showService.GetShowDetailAsync(recentList.First().Id);
            }

            var genreGroups = allShows
                .Where(s => s.Genres != null && s.Genres.Any())
                .GroupBy(s => s.Genres.First().Name)
                .Select(g => new GenreGroupViewModel
                {
                    GenreName = g.Key,
                    Shows = g.Take(6).ToList() 
                })
                .Take(3) 
                .ToList();

            var viewModel = new HomeViewModel
            {
                HeroShow = heroShowDto!,
                PopularShows = popularList,
                RecentShows = recentList,
                GenreGroups = genreGroups
            };

            return View(viewModel);
        }
    }
}