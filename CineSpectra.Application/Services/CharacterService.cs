using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace CineSpectra.Application.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IShowRatingService _ratingService;

        public CharacterService(ICharacterRepository characterRepository, IShowRatingService ratingService)
        {
            _characterRepository = characterRepository;
            _ratingService = ratingService;
        }

        public async Task<CharacterDetailDto?> GetCharacterDetailAsync(int characterId)
        {
            var character = await _characterRepository.GetCharacterWithActorAndShowsAsync(characterId);
            if (character == null) return null;

            var showCredits = character.Shows
                .Select(s => new CharacterShowCreditDto
                {
                    ShowId = s.Id,
                    ShowTitle = s.Title,
                    ShowPosterUrl = s.CoverImageUrl,
                    ShowYear = s.ReleaseDate?.Year,
                    AverageScore = s.CriteriaAverageScore
                })
                .DistinctBy(s => s.ShowId)
                .OrderByDescending(s => s.ShowYear)
                .ToList();

            var ratingStats = await _ratingService.GetShowRatingStatsAsync(characterId);

            // Sadece karakter hedefli kriterleri süzüyoruz
            if (ratingStats?.CriteriaAverages != null)
            {
                ratingStats.CriteriaAverages = ratingStats.CriteriaAverages
                    .Where(c => c.Target == CriteriaTarget.Character || (int)c.Target == 2)
                    .ToList();
            }

            return new CharacterDetailDto
            {
                Id = character.Id,
                Name = character.Name,
                ImageUrl = character.ImageUrl,
                AverageScore = character.AverageScore,
                ActorId = character.ActorId,
                ActorName = character.Actor?.Name,
                Shows = showCredits,
                RatingStats = ratingStats
            };
        }
    }
}