using CineSpectra.Domain.Enums;
using CineSpectra.WebMVC.Models;
using CineSpectra.WebMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineSpectra.WebMVC.Controllers;

public class ShowsController : Controller
{
    private readonly ApiService _apiService;

    public ShowsController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("shows/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var model = await _apiService.GetShowDetailsAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitRating([FromBody] SubmitRatingViewModel ratingModel)
    {
        if (!ModelState.IsValid) return BadRequest();

        var result = await _apiService.SubmitRatingAsync(ratingModel);
        if (result)
        {
            return Ok(new { success = true, message = "Oyunuz başarıyla kaydedildi!" });
        }

        return StatusCode(500, "Oylama kaydedilirken bir hata oluştu.");
    }

    [HttpGet("Shows/{id:int}/Rate")]
    public async Task<IActionResult> Rate(int id)
    {
        var show = await _apiService.GetShowDetailsAsync(id);
        if (show == null) return NotFound();

        var allCriteria = await _apiService.GetAllCriteriaAsync();
        var showCriteria = allCriteria
            .Where(c => c.Target == CriteriaTarget.Show || (int)c.Target == 1)
            .Select(c => new CriteriaVoteItemViewModel
            {
                CriteriaId = c.Id,
                Name = c.Name,
                Weight = c.Weight,
                SelectedScore = 8
            }).ToList();

        var model = new ShowRateViewModel
        {
            ShowId = show.Id,
            Title = show.Title,
            CoverImageUrl = show.CoverImageUrl,
            ReleaseYear = show.ReleaseDate?.Year,
            CurrentAverageScore = show.AverageScore,
            Type = (MediaType)show.Type,
            Criterias = showCriteria,
            Seasons = show.Seasons?.Select(s => new SeasonRateItemDto
            {
                SeasonId = s.Id,
                SeasonNumber = s.SeasonNumber,
                SeasonName = s.Name,
                CurrentAverageScore = s.AverageScore,
                Episodes = s.Episodes?.Select(e => new EpisodeRateItemDto
                {
                    EpisodeId = e.Id,
                    EpisodeNumber = e.EpisodeNumber,
                    Title = e.Title,
                    CurrentAverageScore = e.AverageScore
                }).ToList() ?? new()
            }).ToList() ?? new()
        };

        return View(model);
    }


    [HttpPost("Shows/SubmitFullRating")]
    public async Task<IActionResult> SubmitFullRating([FromBody] SubmitFullRatingInputModel input)
    {
        if (!ModelState.IsValid) return BadRequest();

        var result = await _apiService.SubmitFullRatingAsync(input);
        if (result)
        {
            return Ok(new { success = true });
        }

        return StatusCode(500, "Oylama veritabanına işlenemedi.");
    }


    [HttpPost("Shows/SubmitInlineEpisodeRating")]
    public async Task<IActionResult> SubmitInlineEpisodeRating([FromBody] InlineEntityVoteInputModel input)
    {
        if (!ModelState.IsValid || input.EpisodeId <= 0)
            return BadRequest(new { success = false, message = "Geçersiz bölüm bilgisi." });

        var (isSuccess, message) = await _apiService.SubmitEpisodeRatingDirectAsync(input.ShowId, input.EpisodeId, input.Score);

        if (isSuccess)
            return Ok(new { success = true, message });

        return BadRequest(new { success = false, message });
    }

    [HttpGet("Shows/{id:int}/RateEpisode")]
    public async Task<IActionResult> RateEpisode(int id)
    {
        var show = await _apiService.GetShowDetailsAsync(id);
        if (show == null) return NotFound();

        var model = new ShowRateViewModel
        {
            ShowId = show.Id,
            Title = show.Title,
            CoverImageUrl = show.CoverImageUrl,
            ReleaseYear = show.ReleaseDate?.Year,
            CurrentAverageScore = show.AverageScore,
            Type = (MediaType)show.Type,
            Seasons = show.Seasons?.Select(s => new SeasonRateItemDto
            {
                SeasonId = s.Id,
                SeasonNumber = s.SeasonNumber,
                SeasonName = s.Name,
                CurrentAverageScore = s.AverageScore,
                Episodes = s.Episodes?.Select(e => new EpisodeRateItemDto
                {
                    EpisodeId = e.Id,
                    EpisodeNumber = e.EpisodeNumber,
                    Title = e.Title,
                    CurrentAverageScore = e.AverageScore
                }).ToList() ?? new()
            }).ToList() ?? new()
        };

        return View(model);
    }

    public class InlineEntityVoteInputModel
    {
        public int ShowId { get; set; }
        //public int SeasonId { get; set; }
        public int EpisodeId { get; set; }
        public double Score { get; set; }
    }

}
