using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using CineSpectra.Domain.Enums;
using CineSpectra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CineSpectra.Infrastructure.Repositories
{
    public class ActorRepository : GenericRepository<Actor>, IActorRepository
    {
        public ActorRepository(CineSpectraDbContext context) : base(context)
        {
        }

        public async Task<Actor?> GetActorWithCharactersAndShowsAsync(int actorId)
        {
            return await _context.Actors
                .Include(a => a.Characters)
                    .ThenInclude(c => c.Shows)
                .FirstOrDefaultAsync(a => a.Id == actorId);
        }
    }
}