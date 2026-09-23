using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineSpectra.Application.Services
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _actorRepository;
        private readonly IShowRatingService _ratingService;
        private readonly IUnitOfWork _unitOfWork; // Yorumları çekmek için eklendi

        public ActorService(
            IActorRepository actorRepository,
            IShowRatingService ratingService,
            IUnitOfWork unitOfWork)
        {
            _actorRepository = actorRepository;
            _ratingService = ratingService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActorDetailDto?> GetActorDetailAsync(int actorId)
        {
            var actor = await _actorRepository.GetActorWithCharactersAndShowsAsync(actorId);
            if (actor == null) return null;

            // 1. Filmografi Listesini Hazırla
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

            // 2. Kriter Ortalamalarını Çek ve Filtrele
            var ratingStats = await _ratingService.GetShowRatingStatsAsync(actorId);

            if (ratingStats?.CriteriaAverages != null)
            {
                ratingStats.CriteriaAverages = ratingStats.CriteriaAverages
                    .Where(c => c.Target == CriteriaTarget.Actor || (int)c.Target == 3)
                    .ToList();
            }

            // 3. Aktöre Yapılan Yazılı Yorumları Çek
            var actorRatings = await _unitOfWork.ActorRatings
                .FindAsync(r => r.ActorId == actorId && !string.IsNullOrWhiteSpace(r.Comment));

            var comments = actorRatings
                .GroupBy(r => new { r.UserId, r.Comment }) // Aynı oylamada birden fazla kriter satırı olabileceği için tekilleştiriyoruz
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

            return new ActorDetailDto
            {
                Id = actor.Id,
                Name = actor.Name,
                Biography = actor.Biography,
                ProfileImageUrl = actor.ProfileImageUrl,
                BirthDate = actor.BirthDate,
                AverageScore = actor.AverageScore,
                Filmography = filmography,
                RatingStats = ratingStats,
                Comments = comments // Yorum listesi bağlandı
            };
        }
    }
}