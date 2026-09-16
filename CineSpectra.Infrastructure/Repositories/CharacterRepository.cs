using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using CineSpectra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CineSpectra.Infrastructure.Repositories
{
    public class CharacterRepository : GenericRepository<Character>, ICharacterRepository
    {
        public CharacterRepository(CineSpectraDbContext context) : base(context)
        {
        }

        public async Task<Character?> GetCharacterWithActorAndShowsAsync(int characterId)
        {
            return await _context.Characters
                .Include(c => c.Actor)
                .Include(c => c.Shows)
                .FirstOrDefaultAsync(c => c.Id == characterId);
        }
    }
}