using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineSpectra.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class RatingsController : ControllerBase
    {
        private readonly IShowRatingService _ratingService;

        public RatingsController(IShowRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // =========================================================================
        // 1. KRİTERLERİ LİSTELEME (Anonim erişime açık)
        // =========================================================================
        [AllowAnonymous]
        [HttpGet("criteria")]
        public async Task<IActionResult> GetRatingCriteria()
        {
            var criteria = await _ratingService.GetAllCriteriaAsync();
            return Ok(criteria);
        }

        // =========================================================================
        // 2. DİZİ / FİLM GENEL VEYA ALT KRİTER OYLAMASI
        // =========================================================================
        [HttpPost("show-full")]
        public async Task<IActionResult> SubmitFullRating([FromBody] CreateShowRatingDto ratingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized(new { message = "Oy kullanabilmek için giriş yapmalısınız." });

            ratingDto.UserId = currentUserId;

            var result = await _ratingService.SubmitShowRatingAsync(ratingDto);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(new { message = result.Message });
        }

        // Geriye dönük uyumluluk endpoint'i
        [HttpPost]
        public async Task<IActionResult> SubmitRating([FromBody] CreateShowRatingDto ratingDto)
        {
            return await SubmitFullRating(ratingDto);
        }


        // =========================================================================
        // 4. BÖLÜM OYLAMASI 
        // =========================================================================
        [HttpPost("episode")]
        public async Task<IActionResult> SubmitEpisodeRating([FromBody] SubmitEntityRatingDto dto)
        {
            if (dto.EpisodeId <= 0 || dto.ShowId <= 0)
                return BadRequest(new { message = "Geçerli bir EpisodeId ve ShowId belirtilmelidir." });

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized(new { message = "Oy kullanabilmek için giriş yapmalısınız." });

            var result = await _ratingService.SubmitEpisodeRatingAsync(
                dto.EpisodeId,
                dto.ShowId,
                dto.Score,
                currentUserId);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(new { message = result.Message });
        }

        // =========================================================================
        // 5. BİR YAPIMIN İSTATİSTİKLERİ (Anonim erişime açık)
        // =========================================================================
        [AllowAnonymous]
        [HttpGet("show/{showId:int}")]
        public async Task<IActionResult> GetShowRatingStats(int showId)
        {
            var stats = await _ratingService.GetShowRatingStatsAsync(showId);

            if (stats == null || stats.TotalVotes == 0)
                return NotFound(new { message = "Bu yapıma ait oylama verisi bulunamadı." });

            return Ok(stats);
        }

        // -------------------------------------------------------------------------
        // YARDIMCI METOTLAR
        // -------------------------------------------------------------------------
        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
        }
    }

    public class SubmitEntityRatingDto
    {
        public int ShowId { get; set; }
        public int SeasonId { get; set; }
        public int EpisodeId { get; set; }
        public double Score { get; set; }
        public string? UserId { get; set; }
    }
}