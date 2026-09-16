using Microsoft.AspNetCore.Mvc;
using CineSpectra.Application.Interfaces;
using CineSpectra.Application.DTOs;
using System.Security.Claims;

namespace CineSpectra.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IShowRatingService _ratingService;

        private const string DEFAULT_TEST_USER_ID = "test-user-1";

        public RatingsController(IShowRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // =========================================================================
        // 1. KRİTERLERİ LİSTELEME
        // =========================================================================
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

            // Giriş yapmış kullanıcı varsa ID'sini token/claim üzerinden bağla
            ratingDto.UserId ??= GetCurrentUserId();

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
        // 3. SEZON OYLAMASI (Bölüm oyu varsa engellenir)
        // =========================================================================
        [HttpPost("season")]
        public async Task<IActionResult> SubmitSeasonRating([FromBody] SubmitEntityRatingDto dto)
        {
            if (dto.SeasonId <= 0 || dto.ShowId <= 0)
                return BadRequest(new { message = "Geçerli bir SeasonId ve ShowId belirtilmelidir." });

            string userId = dto.UserId ?? GetCurrentUserId() ?? "test-user-1";


            var result = await _ratingService.SubmitSeasonRatingAsync(
                dto.SeasonId,
                dto.ShowId,
                dto.Score,
                userId);

            if (result.IsSuccess)
                return Ok(result);

            // Kural ihlalinde (bölümler daha önce oylandıysa) 400 Bad Request döner
            return BadRequest(new { message = result.Message });
        }

        // =========================================================================
        // 4. BÖLÜM OYLAMASI (Sezon oyu varsa engellenir)
        // =========================================================================
        [HttpPost("episode")]
        public async Task<IActionResult> SubmitEpisodeRating([FromBody] SubmitEntityRatingDto dto)
        {
            if (dto.EpisodeId <= 0 || dto.ShowId <= 0)
                return BadRequest(new { message = "Geçerli bir EpisodeId ve ShowId belirtilmelidir." });

            string userId = dto.UserId ?? GetCurrentUserId() ?? "test-user-1";

            var result = await _ratingService.SubmitEpisodeRatingAsync(
                dto.EpisodeId,
                dto.ShowId,
                dto.Score,
                userId);

            if (result.IsSuccess)
                return Ok(result);

            // Kural ihlalinde (sezonun geneli daha önce oylandıysa) 400 Bad Request döner
            return BadRequest(new { message = result.Message });
        }

        // =========================================================================
        // 5. BİR YAPIMIN DETAYLI İSTATİSTİKLERİ VE KRİTER ORTALAMALARI
        // =========================================================================
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

    // Sezon ve Bölüm isteklerini karşılayan ortak DTO modeli
    public class SubmitEntityRatingDto
    {
        public int ShowId { get; set; }
        public int SeasonId { get; set; }
        public int EpisodeId { get; set; }
        public double Score { get; set; }
        public string? UserId { get; set; }
    }
}