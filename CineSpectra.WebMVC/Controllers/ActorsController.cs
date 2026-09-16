using Microsoft.AspNetCore.Mvc;
using CineSpectra.WebMVC.Services;
using CineSpectra.WebMVC.Models;

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
}