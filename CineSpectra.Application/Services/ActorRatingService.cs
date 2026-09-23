using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CineSpectra.Application.Services
{
    public class ActorRatingService : IActorRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActorRatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RatingResultDto> SubmitActorRatingAsync(CreateActorRatingDto dto)
        {

            var actor = await _unitOfWork.Actors.GetByIdAsync(dto.ActorId);
            if (actor == null)
                return new RatingResultDto { IsSuccess = false, Message = "Aktör bulunamadı." };

            // 1. Kullanıcının bu aktöre daha önceden verdiği oyları çek
            var userExistingRatings = (await _unitOfWork.ActorRatings
                .FindAsync(r => r.ActorId == dto.ActorId && r.UserId == dto.UserId))
                .ToList();

            // 2. Varsa eski oyları temizle (Upsert / Güncelleme mantığı)
            if (userExistingRatings.Any())
            {
                foreach (var oldVote in userExistingRatings)
                {
                    _unitOfWork.ActorRatings.Delete(oldVote);
                }
            }

            // 3. Yeni oyları ActorRating tablosuna ekle
            if (dto.SubRatings != null && dto.SubRatings.Any())
            {
                // Detaylı kriter oylaması: her kriter için bir ActorRating satırı
                foreach (var sub in dto.SubRatings)
                {
                    await _unitOfWork.ActorRatings.AddAsync(new ActorRating
                    {
                        ActorId = dto.ActorId,
                        UserId = dto.UserId,
                        CriteriaId = sub.CriteriaId,
                        Score = (int)Math.Round(sub.Score),
                        Comment = dto.Comment
                    });
                }
            }
            else
            {
                // Hızlı oylama (CriteriaId belirtilmemişse 0 veya genel bir kriter ID'si)
                await _unitOfWork.ActorRatings.AddAsync(new ActorRating
                {
                    ActorId = dto.ActorId,
                    UserId = dto.UserId,
                    CriteriaId = null,
                    Score = (int)Math.Round(dto.OverallScore),
                    Comment = dto.Comment
                });
            }

            await _unitOfWork.SaveChangesAsync();

            // 4. VoteCount olmadan Aktörün yeni AverageScore değerini hesaplama:
            // Aktöre ait tüm kullanıcı oylarını çekip ortalamasını alıyoruz
            var allActorRatings = (await _unitOfWork.ActorRatings
                .FindAsync(r => r.ActorId == dto.ActorId))
                .ToList();

            if (allActorRatings.Any())
            {
                // Kullanıcı bazlı genel puanları gruplayıp aktörün ortalamasını güncelliyoruz
                var userAverages = allActorRatings
                    .GroupBy(r => r.UserId)
                    .Select(g => g.Average(x => x.Score));

                actor.AverageScore = Math.Round(userAverages.Average(), 1);
                _unitOfWork.Actors.Update(actor);
                await _unitOfWork.SaveChangesAsync();
            }

            return new RatingResultDto
            {
                IsSuccess = true,
                NewAverageScoreOfShow = actor.AverageScore,
                Message = "Aktör oylamanız başarıyla kaydedildi."
            };
        }
    }
}