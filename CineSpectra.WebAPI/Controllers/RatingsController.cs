using Microsoft.AspNetCore.Mvc;
using CineSpectra.Application.Interfaces;
using CineSpectra.Application.DTOs;

namespace CineSpectra.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IShowRatingService _ratingService; 

        public RatingsController(IShowRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRating([FromBody] CreateShowRatingDto ratingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _ratingService.SubmitShowRatingAsync(ratingDto);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result.Message);
        }

        [HttpGet("criteria")]
        public async Task<IActionResult> GetRatingCriteria()
        {
            var criteria = await _ratingService.GetAllCriteriaAsync();
            return Ok(criteria);
        }

        [HttpGet("show/{showId}")]
        public async Task<IActionResult> GetShowRatingStats(int showId)
        {
            var stats = await _ratingService.GetShowRatingStatsAsync(showId);

            if (stats == null || stats.TotalVotes == 0)
                return NotFound("Bu yapıma ait oylama verisi bulunamadı.");

            return Ok(stats);
        }
    }
}