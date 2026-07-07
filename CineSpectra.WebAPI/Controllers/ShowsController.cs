using Microsoft.AspNetCore.Mvc;
using CineSpectra.Application.Interfaces;
using System.Threading.Tasks;

namespace CineSpectra.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularShows([FromQuery] int count = 10)
        {
            var result = await _showService.GetPopularShowsAsync(count);
            return Ok(result);
        }

        [HttpGet("movies")]
        public async Task<IActionResult> GetMovies()
        {
            var result = await _showService.GetMoviesAsync();
            return Ok(result);
        }

        [HttpGet("tvshows")]
        public async Task<IActionResult> GetTvShows()
        {
            var result = await _showService.GetTvShowsAsync();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchShows([FromQuery] string term)
        {
            var result = await _showService.SearchAsync(term);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowDetail(int id)
        {
            var result = await _showService.GetShowDetailAsync(id);

            if (result == null)
                return NotFound(new { Message = $"{id} ID'li yapım veritabanında bulunamadı." });

            return Ok(result);
        }
    }
}