using CineSpectra.Domain.Enums;
using CineSpectra.WebMVC.Models;
using CineSpectra.WebMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineSpectra.WebMVC.Controllers;

public class ActorsController : Controller
{
    private readonly ApiService _apiService;

    public ActorsController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("actors/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var model = await _apiService.GetActorDetailsAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitRating([FromBody] SubmitActorRatingViewModel ratingModel)
    {
        if (!ModelState.IsValid) return BadRequest();

        var result = await _apiService.SubmitActorRatingAsync(ratingModel);
        if (result)
        {
            return Ok(new { success = true, message = "Oyuncuya verdiğiniz oy başarıyla kaydedildi!" });
        }

        return StatusCode(500, "Oylama kaydedilirken bir hata oluştu.");
    }

    [HttpGet("Actors/{id:int}/Rate")]
    public async Task<IActionResult> Rate(int id)
    {
        var actor = await _apiService.GetActorDetailsAsync(id);
        if (actor == null) return NotFound();

        // Sadece CriteriaTarget == Actor (3) olan kriterleri alıyoruz
        var allCriteria = await _apiService.GetAllCriteriaAsync();
        var actorCriteria = allCriteria
            .Where(c => c.Target == CriteriaTarget.Actor || (int)c.Target == 3)
            .Select(c => new ActorCriteriaVoteItemViewModel
            {
                CriteriaId = c.Id,
                Name = c.Name,
                Weight = c.Weight
            }).ToList();

        var model = new ActorRateViewModel
        {
            ActorId = actor.Id,
            Name = actor.Name,
            ProfileImageUrl = actor.ProfileImageUrl,
            Age = actor.BirthDate.HasValue ? DateTime.Today.Year - actor.BirthDate.Value.Year : null,
            CurrentAverageScore = actor.AverageScore,
            Criterias = actorCriteria
        };

        return View(model);
    }

    [HttpPost("Actors/SubmitRating")]
    public async Task<IActionResult> SubmitRatingDetailed([FromBody] SubmitActorRatingInputModel input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _apiService.SubmitActorRatingAsync(input);
        if (result)
        {
            return Ok(new { success = true, message = "Oylamanız başarıyla kaydedildi!" });
        }

        return StatusCode(500, new { success = false, message = "Oylama kaydedilemedi." });
    }
}