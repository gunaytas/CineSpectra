using AutoMapper;
using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Application.Services
{
    public class ShowService : IShowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IShowRatingService _ratingService;

        public ShowService(IUnitOfWork unitOfWork, IMapper mapper, IShowRatingService ratingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ratingService = ratingService;
        }

        public async Task<IEnumerable<ShowListDto>> GetPopularShowsAsync(int count)
        {
            var shows = await _unitOfWork.Shows.GetPopularShowsAsync(count);
            return _mapper.Map<IEnumerable<ShowListDto>>(shows);
        }

        public async Task<IEnumerable<ShowListDto>> GetMoviesAsync()
        {
            var movies = await _unitOfWork.Shows.GetAllMoviesAsync();
            return _mapper.Map<IEnumerable<ShowListDto>>(movies);
        }

        public async Task<IEnumerable<ShowListDto>> GetTvShowsAsync()
        {
            var tvShows = await _unitOfWork.Shows.GetAllTvShowsAsync();
            return _mapper.Map<IEnumerable<ShowListDto>>(tvShows);
        }

        public async Task<IEnumerable<ShowListDto>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<ShowListDto>();

            var results = await _unitOfWork.Shows.SearchShowsAsync(searchTerm);
            return _mapper.Map<IEnumerable<ShowListDto>>(results);
        }

        public async Task<ShowDetailDto?> GetShowDetailAsync(int id, string? userId = null)
        {
            var show = await _unitOfWork.Shows.GetShowDetailWithCastAndGenresAsync(id);

            if (show == null) return null;

            var showDto = _mapper.Map<ShowDetailDto>(show);

            // 1. Kriter istatistiklerini bağla
            showDto.RatingStats = await _ratingService.GetShowRatingStatsAsync(id);

            Dictionary<int, double> userEpisodeRatings = new();
            if (!string.IsNullOrEmpty(userId))
            {
                var userRatings = await _unitOfWork.MediaRatings.FindAsync(r =>
                    r.ShowId == id &&
                    r.UserId == userId &&
                    r.EpisodeId != null);

                userEpisodeRatings = userRatings
                    .ToDictionary(r => r.EpisodeId!.Value, r => r.CalculatedRatingValue);
            }


            if (showDto.Seasons != null && showDto.Seasons.Any()) 
            {
                double totalAudienceScoreSum = 0; 
                int totalAudienceVoteCount = 0; 

                foreach (var seasonDto in showDto.Seasons) 
                {
                    var entitySeason = show.Seasons.FirstOrDefault(s => s.Id == seasonDto.Id); 
                    if (entitySeason == null) continue; 

                    double directSeasonScore = entitySeason.AverageScore; 
                    int directSeasonVotes = entitySeason.VoteCount; 

                    var ratedEpisodes = entitySeason.Episodes ? 
                        .Where(e => e.VoteCount > 0)
                        .ToList() ?? new List<Episode>(); 

                    double episodesTotalScoreSum = ratedEpisodes.Sum(e => e.AverageScore * e.VoteCount); 
                    int episodesTotalVotes = ratedEpisodes.Sum(e => e.VoteCount);

                    int combinedSeasonVotes = directSeasonVotes + episodesTotalVotes; 
                    if (combinedSeasonVotes > 0) 
                    {
                        double combinedSeasonScore = (directSeasonScore * directSeasonVotes + episodesTotalScoreSum) / combinedSeasonVotes; 
                        seasonDto.AverageScore = Math.Round(combinedSeasonScore, 1); 
                    }
                    else
                    {
                        seasonDto.AverageScore = 0.0; 
                    }
                    
                    totalAudienceScoreSum += (directSeasonScore * directSeasonVotes) + episodesTotalScoreSum; 
                    totalAudienceVoteCount += combinedSeasonVotes;

                    // 👇 Bölümlere ait kullanıcı oylarını (UserScore) burada dolduruyoruz
                    if (seasonDto.Episodes != null && seasonDto.Episodes.Any())
                    {
                        foreach (var episodeDto in seasonDto.Episodes)
                        {
                            if (userEpisodeRatings.TryGetValue(episodeDto.Id, out var score))
                            {
                                episodeDto.UserScore = score;
                            }
                        }
                    }
                }
                
                if (totalAudienceVoteCount > 0)
                {
                    showDto.EpisodeAudienceScore = Math.Round(totalAudienceScoreSum / totalAudienceVoteCount, 1); 
                }
            }

            // 3. MediaRatings tablosundan yapıma ait yazılı yorumları çek ve bağla
            var ratings = await _unitOfWork.MediaRatings.GetRatingsByShowIdWithCriteriaAsync(id);
            if (ratings != null && ratings.Any())
            {
                showDto.Comments = ratings
                    .Where(r => !string.IsNullOrWhiteSpace(r.Comment))
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ShowCommentDto
                    {
                        UserId = r.UserId,
                        UserName = !string.IsNullOrEmpty(r.UserId) ? r.UserId : "Anonim İzleyici",
                        Score = r.CalculatedRatingValue,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToList();
            }

            return showDto;
        }
    }
}