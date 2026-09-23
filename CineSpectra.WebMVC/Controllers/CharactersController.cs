using CineSpectra.Domain.Enums;
using CineSpectra.WebMVC.Models;
using CineSpectra.WebMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CineSpectra.WebMVC.Controllers;

public class CharactersController : Controller
{
    private readonly ApiService _apiService;

    public CharactersController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("characters/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var model = await _apiService.GetCharacterDetailsAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        return View(model);
    }

    [HttpGet("Characters/{id:int}/Rate")]
    public async Task<IActionResult> Rate(int id)
    {
        var character = await _apiService.GetCharacterDetailsAsync(id);
        if (character == null) return NotFound();

        var allCriteria = await _apiService.GetAllCriteriaAsync();
        var characterCriteria = allCriteria
            .Where(c => c.Target == CriteriaTarget.Character || (int)c.Target == 4)
            .Select(c => new CharacterCriteriaVoteItemViewModel
            {
                CriteriaId = c.Id,
                Name = c.Name,
                Weight = c.Weight
            }).ToList();

        var model = new CharacterRateViewModel
        {
            CharacterId = character.Id,
            Name = character.Name,
            ImageUrl = character.ImageUrl,
            ActorName = character.ActorName,
            ActorId = character.ActorId,
            CurrentAverageScore = character.AverageScore,
            Criterias = characterCriteria
        };

        return View(model);
    }

    [HttpPost("Characters/SubmitRating")]
    public async Task<IActionResult> SubmitRating([FromBody] SubmitCharacterRatingInputModel input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _apiService.SubmitCharacterFullRatingAsync(input);
        if (result)
        {
            return Ok(new { success = true });
        }

        return StatusCode(500, new { success = false, message = "Karakter oylaması kaydedilemedi." });
    }
}