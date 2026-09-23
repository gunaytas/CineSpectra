using System.Security.Claims;
using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineSpectra.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActorRatingsController : ControllerBase
{
    private readonly IActorRatingService _actorRatingService;
    private readonly IUnitOfWork _unitOfWork;

    public ActorRatingsController(IActorRatingService actorRatingService, IUnitOfWork unitOfWork)
    {
        _actorRatingService = actorRatingService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Aktöre ait hızlı veya kriter bazlı tam oylamayı kaydeder.
    /// </summary>
    [HttpPost("full")]
    public async Task<IActionResult> SubmitFullRating([FromBody] CreateActorRatingDto ratingDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        ratingDto.UserId ??= GetCurrentUserId();

        var result = await _actorRatingService.SubmitActorRatingAsync(ratingDto);
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(new { message = result.Message });
    }

    /// <summary>
    /// Aktörün toplam oy sayısını ve kriter bazlı puan ortalamalarını getirir.
    /// </summary>
    [HttpGet("actor/{actorId:int}")]
    public async Task<IActionResult> GetActorRatingStats(int actorId)
    {
        var ratings = await _unitOfWork.ActorRatings.FindAsync(r => r.ActorId == actorId);
        var ratingList = ratings.ToList();

        if (!ratingList.Any())
        {
            return Ok(new ShowRatingStatsDto
            {
                TotalVotes = 0,
                CriteriaAverages = new List<CriteriaAverageDto>()
            });
        }

        // Tekil oy kullanan kullanıcı sayısı
        int totalVotes = ratingList.Select(r => r.UserId).Distinct().Count();

        // Kriter bazlı oy kullanan kullanıcı sayısı
        var allCriteria = await _unitOfWork.RatingCriterias.GetAllAsync();
        var criteriaMap = allCriteria.ToDictionary(c => c.Id, c => c.Name);

        var criteriaAverages = ratingList
            .Where(r => r.CriteriaId > 0)
            .GroupBy(r => r.CriteriaId!.Value)
            .Select(g => new CriteriaAverageDto
            {
                CriteriaId = g.Key,
                CriteriaName = criteriaMap.TryGetValue(g.Key, out var name) ? name : "Kriter",
                AverageScore = Math.Round(g.Average(x => x.Score), 1)
            })
            .ToList();

        return Ok(new ShowRatingStatsDto
        {
            TotalVotes = totalVotes,
            CriteriaAverages = criteriaAverages
        });
    }

    [HttpGet("user-rating")]
    public async Task<IActionResult> GetUserActorRating([FromQuery] int actorId, [FromQuery] string? userId)
    {
        userId ??= GetCurrentUserId();

        var userRatings = (await _unitOfWork.ActorRatings.FindAsync(r => r.ActorId == actorId && r.UserId == userId)).ToList();
        if (!userRatings.Any())
            return NotFound(new { message = "Bu aktöre ait oylamanız bulunamadı." });

        var isDetailed = userRatings.Any(r => r.CriteriaId > 0);
        double overallScore;

        if (isDetailed)
        {
            overallScore = Math.Round(userRatings.Where(r => r.CriteriaId > 0).Average(r => r.Score), 1);
        }
        else
        {
            overallScore = userRatings.First().Score;
        }

        var dto = new
        {
            ActorId = actorId,
            OverallScore = overallScore,
            Comment = userRatings.FirstOrDefault(r => !string.IsNullOrEmpty(r.Comment))?.Comment,
            SubRatings = userRatings.Where(r => r.CriteriaId > 0).Select(r => new
            {
                CriteriaId = r.CriteriaId,
                Score = r.Score
            }).ToList()
        };

        return Ok(dto);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst("sub")?.Value;
    }
}