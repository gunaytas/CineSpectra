using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;


namespace CineSpectra.Application.Services
{
    public class ShowRatingService : IShowRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowRatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RatingResultDto> SubmitShowRatingAsync(CreateShowRatingDto dto)
        {
            try
            {
                var show = await _unitOfWork.Shows.GetByIdAsync(dto.ShowId);
                if (show == null)
                {
                    return new RatingResultDto
                    {
                        IsSuccess = false, // 👈 Başarısız durum
                        Message = "Oylanmak istenen yapım bulunamadı!"
                    };
                }

                var allCriterias = await _unitOfWork.RatingCriterias.GetAllAsync();
                var criteriaMap = allCriterias.ToDictionary(c => c.Id);

                double totalWeightedScore = 0;
                double totalWeights = 0;


                var subRatingsToSave = new List<MediaRatingSubValue>();

                foreach (var userRating in dto.CriteriaRatings)
                {
                    if (!criteriaMap.TryGetValue(userRating.CriteriaId, out var criteria))
                    {
                        continue;
                    }
                    double score = Math.Clamp(userRating.Score, 1.0, 10.0);

                    double criteriaWeight = criteria.Weight <= 0 ? 1.0 : criteria.Weight; 
                    totalWeightedScore += score * criteriaWeight;
                    totalWeights += criteriaWeight;

                    totalWeightedScore += score * criteria.Weight;
                    totalWeights += criteria.Weight;

                    subRatingsToSave.Add(new MediaRatingSubValue
                    {
                        CriteriaId = criteria.Id,
                        Score = (int)score
                    });
                }

                if (totalWeights == 0)
                {
                    return new RatingResultDto
                    {
                        IsSuccess = false, 
                        Message = "Geçerli hiçbir kriter puanı gönderilmedi!"
                    };
                }

                double finalCalculatedRatingValue = Math.Round(totalWeightedScore / totalWeights, 2);

                var mainShowRating = new MediaRating
                {
                    ShowId = dto.ShowId,
                    CalculatedRatingValue = finalCalculatedRatingValue,
                    CreatedAt = DateTime.UtcNow,
                    SubValues = subRatingsToSave
                };

                await _unitOfWork.MediaRatings.AddAsync(mainShowRating);

                var existingRatings = await _unitOfWork.MediaRatings.GetAllAsync();
                int previousVoteCount = existingRatings.Count(r => r.ShowId == dto.ShowId);

                double currentAverage = show.AverageScore;

                double newAverageScore = ((currentAverage * previousVoteCount) + finalCalculatedRatingValue) / (previousVoteCount + 1);

                show.AverageScore = Math.Round(newAverageScore, 2);

                await _unitOfWork.SaveChangesAsync();

                return new RatingResultDto
                {
                    IsSuccess = true, // 👈 Başarılı durum
                    ShowId = show.Id,
                    CalculatedWeightedScore = finalCalculatedRatingValue,
                    NewAverageScoreOfShow = show.AverageScore,
                    Message = "Oylamanız başarıyla kaydedildi ve yapım puanı güncellendi."
                };
            }
            catch (Exception ex)
            {
                // 👈 Eksik olan catch bloğu eklendi
                return new RatingResultDto
                {
                    IsSuccess = false,
                    Message = $"Oylama sırasında sistemsel bir hata oluştu: {ex.Message}"
                };
            }
        }

        public async Task<List<RatingCriteriaDto>> GetAllCriteriaAsync()
        {
            var criteriaList = await _unitOfWork.RatingCriterias.GetAllAsync();

            return criteriaList.Select(c => new RatingCriteriaDto
            {
                Id = c.Id,
                Name = c.Name,
                Weight = c.Weight,
                Target = c.Target
            }).ToList();
        }

        public async Task<ShowRatingStatsDto> GetShowRatingStatsAsync(int showId)
        {
            // 1. Yeni yazdığımız repository metodunu çağırıyoruz
            var ratings = await _unitOfWork.MediaRatings.GetRatingsByShowIdWithCriteriaAsync(showId);

            if (ratings == null || !ratings.Any())
            {
                return new ShowRatingStatsDto
                {
                    ShowId = showId,
                    OverallAverageScore = 0,
                    TotalVotes = 0,
                    CriteriaAverages = new List<CriteriaAverageDto>()
                };
            }

            // 2. Genel ortalama ve toplam oy sayısını hesaplayalım
            double overallAvg = ratings.Average(r => r.CalculatedRatingValue);
            int totalVotes = ratings.Count;

            // 3. Alt kriterlerin tek tek ortalamalarını hesaplayalım
            var criteriaAverages = ratings
                .SelectMany(r => r.SubValues)
                .GroupBy(sv => new { sv.CriteriaId, sv.Criteria.Name })
                .Select(group => new CriteriaAverageDto
                {
                    CriteriaId = group.Key.CriteriaId,
                    CriteriaName = group.Key.Name,
                    AverageScore = Math.Round(group.Average(g => g.Score), 2)
                })
                .ToList();

            return new ShowRatingStatsDto
            {
                ShowId = showId,
                OverallAverageScore = Math.Round(overallAvg, 2),
                TotalVotes = totalVotes,
                CriteriaAverages = criteriaAverages
            };
        }


    }
}