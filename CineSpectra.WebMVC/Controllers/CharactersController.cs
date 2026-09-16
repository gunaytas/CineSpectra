using Microsoft.AspNetCore.Mvc;
using CineSpectra.WebMVC.Services;
using CineSpectra.WebMVC.Models;
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
}