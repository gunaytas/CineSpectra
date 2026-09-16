using CineSpectra.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineSpectra.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActorDetail(int id)
        {
            var result = await _actorService.GetActorDetailAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"{id} ID'li oyuncu bulunamadı." });
            }
            return Ok(result);
        }
    }
}
