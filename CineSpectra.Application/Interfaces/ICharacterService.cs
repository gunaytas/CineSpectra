using CineSpectra.Application.DTOs;
using System.Threading.Tasks;

namespace CineSpectra.Application.Interfaces
{
    public interface ICharacterService
    {
        Task<CharacterDetailDto?> GetCharacterDetailAsync(int characterId);
    }
}