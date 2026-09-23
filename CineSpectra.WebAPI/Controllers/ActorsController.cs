using AutoMapper;
using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineSpectra.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _actorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActorsController(IActorService actorService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _actorService = actorService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(new List<ActorDto>());

            var actors = await _unitOfWork.Actors.FindAsync(a => EF.Functions.ILike(a.Name, $"%{term}%"));
            return Ok(_mapper.Map<IEnumerable<ActorDto>>(actors));
        }
    }
}
