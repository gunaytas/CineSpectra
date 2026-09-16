using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace CineSpectra.Application.Services
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _actorRepository;
        private readonly IShowRatingService _ratingService;

        public ActorService(IActorRepository actorRepository, IShowRatingService ratingService)
        {
            _actorRepository = actorRepository;
            _ratingService = ratingService;
        }

        public async Task<ActorDetailDto?> GetActorDetailAsync(int actorId)
        {
            var actor = await _actorRepository.GetActorWithCharactersAndShowsAsync(actorId);
            if (actor == null) return null;

            var filmography = actor.Characters
                .Where(c => c.Shows != null && c.Shows.Any())
                .SelectMany(c => c.Shows.Select(s => new ActorCreditDto
                {
                    ShowId = s.Id,
                    ShowTitle = s.Title,
                    ShowPosterUrl = s.CoverImageUrl,
                    ShowYear = s.ReleaseDate?.Year,
                    CharacterId = c.Id,
                    CharacterName = c.Name,
                    AverageScore = s.CriteriaAverageScore
                }))
                .DistinctBy(credit => credit.ShowId)
                .OrderByDescending(credit => credit.ShowYear)
                .ToList();

            var ratingStats = await _ratingService.GetShowRatingStatsAsync(actorId);

            if (ratingStats?.CriteriaAverages != null)
            {
                ratingStats.CriteriaAverages = ratingStats.CriteriaAverages
                    .Where(c => c.Target == CriteriaTarget.Actor || (int)c.Target == 3)
                    .ToList();
            }

            return new ActorDetailDto
            {
                Id = actor.Id,
                Name = actor.Name,
                Biography = actor.Biography,
                ProfileImageUrl = actor.ProfileImageUrl,
                BirthDate = actor.BirthDate,
                AverageScore = actor.AverageScore,
                Filmography = filmography,
                RatingStats = ratingStats
            };
        }
    }
}