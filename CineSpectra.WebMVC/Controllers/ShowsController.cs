using Microsoft.AspNetCore.Mvc;
using CineSpectra.WebMVC.Services;
using CineSpectra.WebMVC.Models;

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
}

// Oylama isteğini taşımak için basit bir model
public class SubmitRatingViewModel
{
    public int ShowId { get; set; }
    public int CriteriaId { get; set; }
    public double Score { get; set; }
}