using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using CineSpectra.Domain.Enums;

namespace CineSpectra.Application.Services
{
    public class ShowRatingServices : IShowRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowRatingServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =========================================================================
        // 1. DİZİ / FİLM KRİTER OYLAMASI (Sanat & Kriter Skoru)
        // =========================================================================
        public async Task<RatingResultDto> SubmitShowRatingAsync(CreateShowRatingDto dto)
        {
            // TEST USER KODU
            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                dto.UserId = "test-user-1";
            }

            var show = await _unitOfWork.Shows.GetByIdAsync(dto.ShowId);
            if (show == null)
                return new RatingResultDto { IsSuccess = false, Message = "Yapım bulunamadı." };

            // Özel repo metodunu çağırıyoruz
            var existingRating = await _unitOfWork.MediaRatings.GetUserShowRatingAsync(dto.ShowId, dto.UserId);

            // Eğer dto.OverallScore gönderilmediyse CriteriaRatings üzerinden hesapla
            double finalScore = dto.OverallScore > 0
                ? dto.OverallScore
                : (dto.CriteriaRatings.Any() ? Math.Round(dto.CriteriaRatings.Average(c => c.Score), 2) : 8.0);

            if (existingRating != null)
            {
                double oldScore = existingRating.CalculatedRatingValue;
                existingRating.CalculatedRatingValue = finalScore;
                existingRating.Comment = dto.Comment;
                existingRating.UpdatedAt = DateTime.UtcNow;

                existingRating.SubValues.Clear();
                if (dto.CriteriaRatings != null && dto.CriteriaRatings.Any())
                {
                    foreach (var sub in dto.CriteriaRatings)
                    {
                        existingRating.SubValues.Add(new MediaRatingSubValue
                        {
                            CriteriaId = sub.CriteriaId,
                            Score = (int)Math.Round(sub.Score)
                        });
                    }
                }

                if (show.CriteriaVoteCount > 0)
                {
                    double totalSum = (show.CriteriaAverageScore * show.CriteriaVoteCount) - oldScore + finalScore;
                    show.CriteriaAverageScore = Math.Round(totalSum / show.CriteriaVoteCount, 2);
                }
            }
            else
            {
                var userRating = new MediaRating
                {
                    ShowId = dto.ShowId,
                    UserId = dto.UserId,
                    Comment = dto.Comment,
                    CalculatedRatingValue = finalScore,
                    CreatedAt = DateTime.UtcNow,
                    SubValues = new List<MediaRatingSubValue>()
                };

                if (dto.CriteriaRatings != null && dto.CriteriaRatings.Any())
                {
                    foreach (var sub in dto.CriteriaRatings)
                    {
                        userRating.SubValues.Add(new MediaRatingSubValue
                        {
                            CriteriaId = sub.CriteriaId,
                            Score = (int)Math.Round(sub.Score)
                        });
                    }
                }

                await _unitOfWork.MediaRatings.AddAsync(userRating);

                double newAvg = ((show.CriteriaAverageScore * show.CriteriaVoteCount) + finalScore)
                                / (show.CriteriaVoteCount + 1);
                show.CriteriaAverageScore = Math.Round(newAvg, 2);
                show.CriteriaVoteCount += 1;
            }

            await _unitOfWork.SaveChangesAsync();

            return new RatingResultDto
            {
                IsSuccess = true,
                ShowId = show.Id,
                CalculatedWeightedScore = finalScore,
                NewAverageScoreOfShow = show.CriteriaAverageScore,
                Message = "Puanınız başarıyla kaydedildi."
            };
        }

        // =========================================================================
        // 2. SEZON OYLAMASI (Bölüm Oyu Varsa Engellenir)
        // =========================================================================
        public async Task<RatingResultDto> SubmitSeasonRatingAsync(int seasonId, int showId, double score, string? userId)
        {

            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = "test-user-1";
            }

            var season = await _unitOfWork.Seasons.GetByIdAsync(seasonId);
            if (season == null)
                return new RatingResultDto { IsSuccess = false, Message = "Sezon bulunamadı." };

            if (!string.IsNullOrEmpty(userId))
            {
                // Kural Kontrolü: Bölüm oyu verilmiş mi?
                bool hasEpisodeRatings = await _unitOfWork.MediaRatings.HasUserRatedAnyEpisodeInSeasonAsync(seasonId, userId);
                if (hasEpisodeRatings)
                {
                    return new RatingResultDto
                    {
                        IsSuccess = false,
                        Message = "Bu sezonun bölümlerini tek tek puanladığınız için sezonun geneline toplu puan veremezsiniz."
                    };
                }
            }

            var existingSeasonRating = await _unitOfWork.MediaRatings.GetUserSeasonRatingAsync(seasonId, userId);

            if (existingSeasonRating != null)
            {
                double oldScore = existingSeasonRating.CalculatedRatingValue;
                existingSeasonRating.CalculatedRatingValue = score;
                existingSeasonRating.UpdatedAt = DateTime.UtcNow;

                if (season.VoteCount > 0)
                {
                    double totalSum = (season.AverageScore * season.VoteCount) - oldScore + score;
                    season.AverageScore = Math.Round(totalSum / season.VoteCount, 2);
                }
            }
            else
            {
                var rating = new MediaRating
                {
                    ShowId = showId,
                    SeasonId = seasonId,
                    UserId = userId,
                    CalculatedRatingValue = score,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.MediaRatings.AddAsync(rating);

                double newSeasonAvg = ((season.AverageScore * season.VoteCount) + score) / (season.VoteCount + 1);
                season.AverageScore = Math.Round(newSeasonAvg, 2);
                season.VoteCount += 1;
            }

            await _unitOfWork.SaveChangesAsync();
            return new RatingResultDto { IsSuccess = true, Message = "Sezon puanınız kaydedildi." };
        }

        // =========================================================================
        // 3. BÖLÜM OYLAMASI (Sezon Oyu Varsa Engellenir)
        // =========================================================================
        public async Task<RatingResultDto> SubmitEpisodeRatingAsync(int episodeId, int showId, double score, string? userId)
        {

            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = "test-user-1";
            }
            var episode = await _unitOfWork.Episodes.GetByIdAsync(episodeId);
            var show = await _unitOfWork.Shows.GetByIdAsync(showId);
            if (episode == null || show == null)
                return new RatingResultDto { IsSuccess = false, Message = "Bölüm veya yapım bulunamadı." };

            if (!string.IsNullOrEmpty(userId))
            {
                // Kural Kontrolü: Sezon oyu verilmiş mi?
                bool hasSeasonRating = await _unitOfWork.MediaRatings.HasUserRatedSeasonAsync(episode.SeasonId, userId);
                if (hasSeasonRating)
                {
                    return new RatingResultDto
                    {
                        IsSuccess = false,
                        Message = "Bu sezonun geneline toplu puan verdiğiniz için bölümleri ayrıca puanlayamazsınız."
                    };
                }
            }

            var existingEpisodeRating = await _unitOfWork.MediaRatings.GetUserEpisodeRatingAsync(episodeId, userId);

            if (existingEpisodeRating != null)
            {
                double oldScore = existingEpisodeRating.CalculatedRatingValue;
                existingEpisodeRating.CalculatedRatingValue = score;
                existingEpisodeRating.UpdatedAt = DateTime.UtcNow;

                if (episode.VoteCount > 0)
                {
                    double totalEpSum = (episode.AverageScore * episode.VoteCount) - oldScore + score;
                    episode.AverageScore = Math.Round(totalEpSum / episode.VoteCount, 2);
                }

                if (show.TotalEpisodeVoteCount > 0 && show.EpisodeAudienceScore.HasValue)
                {
                    double totalAudienceSum = (show.EpisodeAudienceScore.Value * show.TotalEpisodeVoteCount) - oldScore + score;
                    show.EpisodeAudienceScore = Math.Round(totalAudienceSum / show.TotalEpisodeVoteCount, 1);
                }
            }
            else
            {
                var rating = new MediaRating
                {
                    ShowId = showId,
                    SeasonId = episode.SeasonId,
                    EpisodeId = episodeId,
                    UserId = userId,
                    CalculatedRatingValue = score,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.MediaRatings.AddAsync(rating);

                double newEpAvg = ((episode.AverageScore * episode.VoteCount) + score) / (episode.VoteCount + 1);
                episode.AverageScore = Math.Round(newEpAvg, 2);
                episode.VoteCount += 1;

                double currentAudienceScore = show.EpisodeAudienceScore ?? 0.0;
                double newAudienceAvg = ((currentAudienceScore * show.TotalEpisodeVoteCount) + score) / (show.TotalEpisodeVoteCount + 1);
                show.EpisodeAudienceScore = Math.Round(newAudienceAvg, 1);
                show.TotalEpisodeVoteCount += 1;
            }

            await _unitOfWork.SaveChangesAsync();
            return new RatingResultDto { IsSuccess = true, Message = "Bölüm puanınız kaydedildi." };
        }

        // =========================================================================
        // 4. KRİTERLERİ GETİR
        // =========================================================================
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

        // =========================================================================
        // 5. KRİTER BAZLI İSTATİSTİKLER (Detay Sayfası Kriter Barları)
        // =========================================================================
        public async Task<ShowRatingStatsDto> GetShowRatingStatsAsync(int showId)
        {
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

            var criteriaAverages = ratings
                .SelectMany(r => r.SubValues)
                .Where(sv => sv.Criteria != null && (sv.Criteria.Target == CriteriaTarget.Show || (int)sv.Criteria.Target == 1))
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
                OverallAverageScore = Math.Round(ratings.Average(r => r.CalculatedRatingValue), 2),
                TotalVotes = ratings.Count,
                CriteriaAverages = criteriaAverages
            };
        }
    }
}