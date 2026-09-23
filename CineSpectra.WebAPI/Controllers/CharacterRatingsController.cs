using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CineSpectra.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharacterRatingsController : ControllerBase
{
    private readonly ICharacterRatingService _characterRatingService;
    private readonly IUnitOfWork _unitOfWork;

    public CharacterRatingsController(ICharacterRatingService characterRatingService, IUnitOfWork unitOfWork)
    {
        _characterRatingService = characterRatingService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Karakter için hızlı veya alt kriter bazlı oylamayı kaydeder / günceller.
    /// POST: api/CharacterRatings/full
    /// </summary>
    [HttpPost("full")]
    public async Task<IActionResult> SubmitFullRating([FromBody] CreateCharacterRatingDto ratingDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Kullanıcı kimliği claim üzerinden alınır, yoksa test kullanıcısı atanır
        ratingDto.UserId ??= GetCurrentUserId();

        var result = await _characterRatingService.SubmitCharacterRatingAsync(ratingDto);
        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(new { message = result.Message });
    }

    /// <summary>
    /// Karakterin toplam oy sayısını ve Target == Character olan kriter ortalamalarını getirir.
    /// GET: api/CharacterRatings/character/{characterId}
    /// </summary>
    [AllowAnonymous]
    [HttpGet("character/{characterId:int}")]
    public async Task<IActionResult> GetCharacterRatingStats(int characterId)
    {
        var ratings = await _unitOfWork.CharacterRatings.FindAsync(r => r.CharacterId == characterId);
        var ratingList = ratings.ToList();

        if (!ratingList.Any())
        {
            return Ok(new ShowRatingStatsDto
            {
                TotalVotes = 0,
                CriteriaAverages = new List<CriteriaAverageDto>()
            });
        }

        // Tekil oy veren kullanıcı sayısı
        int totalVotes = ratingList.Select(r => r.UserId).Distinct().Count();

        // Kriter adlarını eşleştirmek için kriter tanımlarını çekiyoruz
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
    public async Task<IActionResult> GetUserCharacterRating([FromQuery] int characterId, [FromQuery] string? userId)
    {
        userId ??= GetCurrentUserId();

        var userRatings = (await _unitOfWork.CharacterRatings
            .FindAsync(r => r.CharacterId == characterId && r.UserId == userId))
            .ToList();

        if (!userRatings.Any())
            return NotFound(new { message = "Bu karaktere ait oylamanız bulunamadı." });

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
            CharacterId = characterId,
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