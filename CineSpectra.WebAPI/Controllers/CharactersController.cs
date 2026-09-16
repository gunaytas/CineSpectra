using CineSpectra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineSpectra.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCharacterDetail(int id)
        {
            var result = await _characterService.GetCharacterDetailAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"{id} ID'li karakter bulunamadı." });
            }
            return Ok(result);
        }
    }
}