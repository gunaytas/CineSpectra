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
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CharactersController(ICharacterService characterService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _characterService = characterService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Ok(new List<CharacterDto>());
            var characters = await _unitOfWork.Characters.FindAsync(c => EF.Functions.ILike(c.Name, $"%{term}%"));
            return Ok(_mapper.Map<IEnumerable<CharacterDto>>(characters));
        }
    }
}