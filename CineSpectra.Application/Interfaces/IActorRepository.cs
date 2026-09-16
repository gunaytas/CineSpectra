using CineSpectra.Domain.Entities;
using System.Threading.Tasks;

namespace CineSpectra.Application.Interfaces
{
    public interface IActorRepository : IGenericRepository<Actor> 
    {
        Task<Actor?> GetActorWithCharactersAndShowsAsync(int actorId);
    }
}