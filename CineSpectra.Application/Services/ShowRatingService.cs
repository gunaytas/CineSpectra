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

            var existingEpisodeRating = await _unitOfWork.MediaRatings.GetUserEpisodeRatingAsync(episodeId, userId);

            if (existingEpisodeRating != null)
            {
                double oldScore = existingEpisodeRating.CalculatedRatingValue;
                existingEpisodeRating.CalculatedRatingValue = score;
                existingEpisodeRating.UpdatedAt = DateTime.UtcNow;

                // 1. Bölümün Kendi Ortalamasını Güncelle
                if (episode.VoteCount > 0)
                {
                    double totalEpSum = (episode.AverageScore * episode.VoteCount) - oldScore + score;
                    episode.AverageScore = Math.Round(totalEpSum / episode.VoteCount, 2);
                }

                // 2. Dizinin Genel Seyirci Skorunu Güncelle
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

                // 1. Bölüm Sayacını Güncelle
                double newEpAvg = ((episode.AverageScore * episode.VoteCount) + score) / (episode.VoteCount + 1);
                episode.AverageScore = Math.Round(newEpAvg, 2);
                episode.VoteCount += 1;

                // 2. Dizinin Genel Seyirci Skorunu Güncelle
                double currentAudienceScore = show.EpisodeAudienceScore ?? 0.0;
                double newAudienceAvg = ((currentAudienceScore * show.TotalEpisodeVoteCount) + score) / (show.TotalEpisodeVoteCount + 1);
                show.EpisodeAudienceScore = Math.Round(newAudienceAvg, 1);
                show.TotalEpisodeVoteCount += 1;
            }

            // 3. SEZON ORTALAMASINI OTOMATİK HESAPLA VE KAYDET
            var season = await _unitOfWork.Seasons.GetByIdAsync(episode.SeasonId);
            if (season != null)
            {
                // Sezonun puan almış tüm bölümlerini al
                var seasonEpisodes = await _unitOfWork.Episodes.FindAsync(e => e.SeasonId == season.Id);
                var ratedEpisodes = seasonEpisodes.Where(e => e.VoteCount > 0).ToList();

                if (ratedEpisodes.Any())
                {
                    // Sezon puanı = Bölüm puanlarının ortalaması
                    season.AverageScore = Math.Round(ratedEpisodes.Average(e => e.AverageScore), 1);
                    season.VoteCount = ratedEpisodes.Sum(e => e.VoteCount);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return new RatingResultDto { IsSuccess = true, Message = "Bölüm puanınız kaydedildi." };
        }

        // KRİTERLERİ GETİR
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

        // KRİTER BAZLI İSTATİSTİKLER (Detay Sayfası Kriter Barları)
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

            var calculatedTotalVotes = ratings
        .Where(r => !string.IsNullOrEmpty(r.UserId))
        .GroupBy(r => r.UserId)
        .Sum(userGroup =>
        {
            // Kullanıcı yapım düzeyinde (kriter/genel) oy kullanmış mı?
            bool hasShowRating = userGroup.Any(r => r.SeasonId == null && r.EpisodeId == null);

            // Kullanıcı sezon veya bölüm düzeyinde oy kullanmış mı?
            bool hasSeasonOrEpisodeRating = userGroup.Any(r => r.SeasonId != null || r.EpisodeId != null);

            int voteWeight = 0;
            if (hasShowRating) voteWeight += 1;
            if (hasSeasonOrEpisodeRating) voteWeight += 1;

            return voteWeight;
        });

            // Anonim (UserId boş olan) oylar varsa doğrudan 1 sayılacak şekilde eklenir
            int anonymousVotes = ratings.Count(r => string.IsNullOrEmpty(r.UserId));
            int finalTotalVotes = calculatedTotalVotes + anonymousVotes;

            // 3. Genel ortalama puan (Yalnızca Show düzeyindeki ana puanlardan alınır)
            var showLevelRatings = ratings.Where(r => r.SeasonId == null && r.EpisodeId == null).ToList();
            double overallAvg = showLevelRatings.Any()
                ? Math.Round(showLevelRatings.Average(r => r.CalculatedRatingValue), 2)
                : Math.Round(ratings.Average(r => r.CalculatedRatingValue), 2);

            return new ShowRatingStatsDto
            {
                ShowId = showId,
                OverallAverageScore = overallAvg,
                TotalVotes = finalTotalVotes,
                CriteriaAverages = criteriaAverages
            };
        }
    }
}