using CineSpectra.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IShowRatingRepository MediaRatings { get; }
        IRatingCriteriaRepository RatingCriterias { get; }
        IShowRepository Shows { get; }
        IGenericRepository<Season> Seasons { get; }
        IGenericRepository<Episode> Episodes { get; }
        IActorRepository Actors { get; }
        IGenericRepository<ActorRating> ActorRatings { get; }
        ICharacterRepository Characters { get; }
        IGenericRepository<CharacterRating> CharacterRatings { get; }
        Task<int> SaveChangesAsync();
    }
}