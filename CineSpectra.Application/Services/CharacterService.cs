using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineSpectra.Application.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IShowRatingService _ratingService;
        private readonly IUnitOfWork _unitOfWork; // Yorumları çekmek için eklendi

        public CharacterService(
            ICharacterRepository characterRepository,
            IShowRatingService ratingService,
            IUnitOfWork unitOfWork)
        {
            _characterRepository = characterRepository;
            _ratingService = ratingService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CharacterDetailDto?> GetCharacterDetailAsync(int characterId)
        {
            var character = await _characterRepository.GetCharacterWithActorAndShowsAsync(characterId);
            if (character == null) return null;

            // 1. Karakterin yer aldığı yapımları listele
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

            // 2. Karakter oylama istatistiklerini getir ve filtrele
            var ratingStats = await _ratingService.GetShowRatingStatsAsync(characterId);

            if (ratingStats?.CriteriaAverages != null)
            {
                ratingStats.CriteriaAverages = ratingStats.CriteriaAverages
                    .Where(c => c.Target == CriteriaTarget.Character || (int)c.Target == 2)
                    .ToList();
            }

            // 3. Karaktere ait yazılı yorumları çek
            var characterRatings = await _unitOfWork.CharacterRatings
                .FindAsync(r => r.CharacterId == characterId && !string.IsNullOrWhiteSpace(r.Comment));

            var comments = characterRatings
                .GroupBy(r => new { r.UserId, r.Comment }) // Birden fazla kriter satırı oylanmışsa tekilleştir
                .Select(g => new ShowCommentDto
                {
                    UserId = g.Key.UserId,
                    UserName = !string.IsNullOrEmpty(g.Key.UserId) ? g.Key.UserId : "Anonim İzleyici",
                    Score = Math.Round(g.Average(x => x.Score), 1),
                    Comment = g.Key.Comment,
                    CreatedAt = g.Max(x => x.CreatedAt)
                })
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return new CharacterDetailDto
            {
                Id = character.Id,
                Name = character.Name,
                ImageUrl = character.ImageUrl,
                AverageScore = character.AverageScore,
                ActorId = character.ActorId,
                ActorName = character.Actor?.Name,
                Shows = showCredits,
                RatingStats = ratingStats,
                Comments = comments // Yorum listesi bağlandı
            };
        }
    }
}