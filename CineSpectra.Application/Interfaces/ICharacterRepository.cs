using CineSpectra.Domain.Entities;
using System.Threading.Tasks;

namespace CineSpectra.Application.Interfaces
{
    public interface ICharacterRepository : IGenericRepository<Character>
    {
        Task<Character?> GetCharacterWithActorAndShowsAsync(int characterId);
    }
}