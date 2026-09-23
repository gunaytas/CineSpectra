using System;
using System.Linq;
using System.Threading.Tasks;
using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;

namespace CineSpectra.Application.Services
{
    public class CharacterRatingService : ICharacterRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CharacterRatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RatingResultDto> SubmitCharacterRatingAsync(CreateCharacterRatingDto dto)
        {
            var character = await _unitOfWork.Characters.GetByIdAsync(dto.CharacterId);
            if (character == null)
                return new RatingResultDto { IsSuccess = false, Message = "Karakter bulunamadı." };

            // 1. Kullanıcının daha önceki oylarını bul ve temizle (Upsert)
            var userExistingRatings = (await _unitOfWork.CharacterRatings
                .FindAsync(r => r.CharacterId == dto.CharacterId && r.UserId == dto.UserId))
                .ToList();

            if (userExistingRatings.Any())
            {
                foreach (var oldVote in userExistingRatings)
                {
                    _unitOfWork.CharacterRatings.Delete(oldVote);
                }
            }

            // 2. Yeni oyları kaydet
            if (dto.SubRatings != null && dto.SubRatings.Any())
            {
                foreach (var sub in dto.SubRatings)
                {
                    await _unitOfWork.CharacterRatings.AddAsync(new CharacterRating
                    {
                        CharacterId = dto.CharacterId,
                        UserId = dto.UserId,
                        CriteriaId = sub.CriteriaId,
                        Score = (int)Math.Round(sub.Score),
                        Comment = dto.Comment
                    });
                }
            }
            else
            {
                await _unitOfWork.CharacterRatings.AddAsync(new CharacterRating
                {
                    CharacterId = dto.CharacterId,
                    UserId = dto.UserId,
                    CriteriaId = null,
                    Score = (int)Math.Round(dto.OverallScore),
                    Comment = dto.Comment
                });
            }

            await _unitOfWork.SaveChangesAsync();

            // 3. Karakterin AverageScore değerini güncelle
            var allRatings = (await _unitOfWork.CharacterRatings
                .FindAsync(r => r.CharacterId == dto.CharacterId))
                .ToList();

            if (allRatings.Any())
            {
                var userAverages = allRatings
                    .GroupBy(r => r.UserId)
                    .Select(g => g.Average(x => x.Score));

                character.AverageScore = Math.Round(userAverages.Average(), 1);
                _unitOfWork.Characters.Update(character);
                await _unitOfWork.SaveChangesAsync();
            }

            return new RatingResultDto
            {
                IsSuccess = true,
                NewAverageScoreOfShow = character.AverageScore,
                Message = "Karakter oylamanız başarıyla kaydedildi."
            };
        }
    }
}