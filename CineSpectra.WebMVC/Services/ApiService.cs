using System.Net.Http.Headers;
using System.Net.Http.Json;
using CineSpectra.Application.DTOs;
using CineSpectra.Domain.Enums;
using CineSpectra.WebMVC.Models;
using Microsoft.AspNetCore.Http;
using RatingCriteriaDto = CineSpectra.Application.DTOs.RatingCriteriaDto;

namespace CineSpectra.WebMVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // =========================================================================
        // YARDIMCI: JWT TOKEN EKLEME (Authorization: Bearer <token>)
        // =========================================================================
        private void AttachBearerToken()
        {
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("JwtToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // =========================================================================
        // AUTH İŞLEMLERİ (GİRİŞ & KAYIT)
        // =========================================================================
        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponseDto>()
                           ?? new AuthResponseDto { IsSuccess = false, Message = "Sunucudan boş yanıt döndü." };
                }

                var error = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                return error ?? new AuthResponseDto { IsSuccess = false, Message = "Giriş başarısız oldu." };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Bağlantı hatası: " + ex.Message };
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/register", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponseDto>()
                           ?? new AuthResponseDto { IsSuccess = true };
                }

                var error = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                return error ?? new AuthResponseDto { IsSuccess = false, Message = "Kayıt işlemi başarısız oldu." };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Bağlantı hatası: " + ex.Message };
            }
        }

        // =========================================================================
        // YAPIM (SHOW) İŞLEMLERİ
        // =========================================================================
        public async Task<ShowDetailViewModel?> GetShowDetailsAsync(int showId)
        {
            var showResponse = await _httpClient.GetAsync($"api/Shows/{showId}");
            if (!showResponse.IsSuccessStatusCode) return null;

            var showDto = await showResponse.Content.ReadFromJsonAsync<ShowDetailDto>();
            if (showDto == null) return null;

            var statsResponse = await _httpClient.GetAsync($"api/Ratings/show/{showId}");
            if (statsResponse.IsSuccessStatusCode)
            {
                showDto.RatingStats = await statsResponse.Content.ReadFromJsonAsync<ShowRatingStatsDto>();
            }

            var criteriaResponse = await _httpClient.GetAsync("api/Ratings/criteria");
            var activeCriteria = new List<RatingCriteriaDto>();
            if (criteriaResponse.IsSuccessStatusCode)
            {
                activeCriteria = await criteriaResponse.Content.ReadFromJsonAsync<List<RatingCriteriaDto>>() ?? new();
            }

            var viewModel = new ShowDetailViewModel
            {
                Id = showDto.Id,
                Title = showDto.Title,
                Description = showDto.Description,
                CoverImageUrl = showDto.CoverImageUrl,
                Director = showDto.Director,
                ProductionCompany = showDto.ProductionCompany,
                Writers = showDto.Writers,
                AverageScore = showDto.AverageScore,
                EpisodeAudienceScore = showDto.EpisodeAudienceScore,
                Type = showDto.Type,
                ReleaseDate = showDto.ReleaseDate,
                Seasons = showDto.Seasons,
                Genres = showDto.Genres?.Select(g => new CineSpectra.WebMVC.Models.GenreDto { Id = g.Id, Name = g.Name }).ToList() ?? new(),
                Actors = showDto.Actors?.Select(a => new CineSpectra.WebMVC.Models.ActorDto { Id = a.Id, Name = a.Name, ProfileImageUrl = a.ProfileImageUrl }).ToList() ?? new(),
                Characters = showDto.Characters?.Select(c => new CineSpectra.WebMVC.Models.CharacterDto { Id = c.Id, Name = c.Name, ActorId = c.ActorId, ActorName = c.ActorName, ImageUrl = c.ImageUrl, AverageScore = c.AverageScore }).ToList() ?? new(),
                ActiveCriteria = activeCriteria?.Select(ac => new CineSpectra.WebMVC.Models.RatingCriteriaDto { Id = ac.Id, Name = ac.Name, Weight = ac.Weight, Target = ac.Target }).ToList() ?? new(),
                RatingStats = showDto.RatingStats,
                Comments = showDto.Comments
            };

            return viewModel;
        }

        public async Task<List<ShowListDto>> GetMoviesAsync()
        {
            var response = await _httpClient.GetAsync("api/Shows/movies");
            if (!response.IsSuccessStatusCode)
                return new List<ShowListDto>();

            return await response.Content.ReadFromJsonAsync<List<ShowListDto>>() ?? new List<ShowListDto>();
        }
        public async Task<List<ShowListDto>> GetTvShowsAsync()
        {
            var response = await _httpClient.GetAsync("api/Shows/tvshows");
            if (!response.IsSuccessStatusCode)
                return new List<ShowListDto>();

            return await response.Content.ReadFromJsonAsync<List<ShowListDto>>() ?? new List<ShowListDto>();
        }

        public async Task<bool> SubmitRatingAsync(object ratingDto)
        {
            AttachBearerToken();
            var response = await _httpClient.PostAsJsonAsync("api/Ratings", ratingDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SubmitFullRatingAsync(SubmitFullRatingInputModel input)
        {
            AttachBearerToken();
            var response = await _httpClient.PostAsJsonAsync("api/Ratings/show-full", input);
            return response.IsSuccessStatusCode;
        }

        public async Task<(bool IsSuccess, string Message)> SubmitSeasonRatingDirectAsync(int showId, int seasonId, double score)
        {
            try
            {
                AttachBearerToken();
                var payload = new { showId, seasonId, score };
                var response = await _httpClient.PostAsJsonAsync("api/Ratings/season", payload);

                if (response.IsSuccessStatusCode)
                    return (true, "Sezon puanınız başarıyla kaydedildi.");

                var errorObj = await response.Content.ReadFromJsonAsync<ApiRatingErrorDto>();
                return (false, errorObj?.Message ?? "Sezon puanı işlenirken bir kural hatası oluştu.");
            }
            catch (Exception ex)
            {
                return (false, "Sunucuya bağlanılamadı: " + ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message)> SubmitEpisodeRatingDirectAsync(int showId, int episodeId, double score)
        {
            try
            {
                AttachBearerToken();
                var payload = new { showId, episodeId, score };
                var response = await _httpClient.PostAsJsonAsync("api/Ratings/episode", payload);

                if (response.IsSuccessStatusCode)
                    return (true, "Bölüm puanınız başarıyla kaydedildi.");

                var errorObj = await response.Content.ReadFromJsonAsync<ApiRatingErrorDto>();
                return (false, errorObj?.Message ?? "Bölüm puanı işlenirken bir kural hatası oluştu.");
            }
            catch (Exception ex)
            {
                return (false, "Sunucuya bağlanılamadı: " + ex.Message);
            }
        }

        // =========================================================================
        // AKTÖR (ACTOR) İŞLEMLERİ
        // =========================================================================
        public async Task<ActorDetailViewModel?> GetActorDetailsAsync(int actorId)
        {
            var response = await _httpClient.GetAsync($"api/Actors/{actorId}");
            if (!response.IsSuccessStatusCode) return null;

            var actorViewModel = await response.Content.ReadFromJsonAsync<ActorDetailViewModel>();
            if (actorViewModel == null) return null;

            var statsResponse = await _httpClient.GetAsync($"api/ActorRatings/actor/{actorId}");
            if (statsResponse.IsSuccessStatusCode)
            {
                actorViewModel.RatingStats = await statsResponse.Content.ReadFromJsonAsync<ShowRatingStatsDto>();
            }

            return actorViewModel;
        }

        public async Task<bool> SubmitActorRatingAsync(SubmitActorRatingInputModel input)
        {
            AttachBearerToken();
            var response = await _httpClient.PostAsJsonAsync("api/ActorRatings/full", input);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SubmitActorRatingAsync(SubmitActorRatingViewModel ratingModel)
        {
            var input = new SubmitActorRatingInputModel
            {
                ActorId = ratingModel.ActorId,
                OverallScore = ratingModel.Score,
                IsDetailed = false
            };

            return await SubmitActorRatingAsync(input);
        }

        // =========================================================================
        // KARAKTER (CHARACTER) İŞLEMLERİ
        // =========================================================================
        public async Task<CharacterDetailViewModel?> GetCharacterDetailsAsync(int characterId)
        {
            var response = await _httpClient.GetAsync($"api/Characters/{characterId}");
            if (!response.IsSuccessStatusCode) return null;

            var characterViewModel = await response.Content.ReadFromJsonAsync<CharacterDetailViewModel>();
            if (characterViewModel == null) return null;

            var statsResponse = await _httpClient.GetAsync($"api/CharacterRatings/character/{characterId}");
            if (statsResponse.IsSuccessStatusCode)
            {
                characterViewModel.RatingStats = await statsResponse.Content.ReadFromJsonAsync<ShowRatingStatsDto>();
            }

            return characterViewModel;
        }

        public async Task<bool> SubmitCharacterFullRatingAsync(SubmitCharacterRatingInputModel input)
        {
            AttachBearerToken();
            var response = await _httpClient.PostAsJsonAsync("api/CharacterRatings/full", input);
            return response.IsSuccessStatusCode;
        }

        // =========================================================================
        // KRİTER VE ARAMA SERVİSLERİ
        // =========================================================================
        public async Task<List<CriteriaItemDto>> GetAllCriteriaAsync()
        {
            var response = await _httpClient.GetAsync("api/Ratings/criteria");
            if (!response.IsSuccessStatusCode) return new List<CriteriaItemDto>();

            return await response.Content.ReadFromJsonAsync<List<CriteriaItemDto>>() ?? new List<CriteriaItemDto>();
        }

        public async Task<List<ShowListDto>> SearchShowsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new List<ShowListDto>();

            var response = await _httpClient.GetAsync($"api/Shows/search?term={Uri.EscapeDataString(searchTerm)}");
            if (!response.IsSuccessStatusCode) return new List<ShowListDto>();

            return await response.Content.ReadFromJsonAsync<List<ShowListDto>>() ?? new List<ShowListDto>();
        }

        public async Task<SearchViewModel> MultiSearchAsync(string query, string tab = "all")
        {
            var model = new SearchViewModel { Query = query, ActiveTab = tab };
            if (string.IsNullOrWhiteSpace(query)) return model;

            var encoded = Uri.EscapeDataString(query);

            var showsTask = _httpClient.GetFromJsonAsync<List<ShowListDto>>($"api/Shows/search?term={encoded}");
            var actorsTask = _httpClient.GetFromJsonAsync<List<Application.DTOs.ActorDto>>($"api/Actors/search?term={encoded}");
            var charactersTask = _httpClient.GetFromJsonAsync<List<Application.DTOs.CharacterDto>>($"api/Characters/search?term={encoded}");

            await Task.WhenAll(showsTask, actorsTask, charactersTask);

            model.Shows = (await showsTask) ?? new();
            model.Actors = (await actorsTask) ?? new();
            model.Characters = (await charactersTask) ?? new();

            return model;
        }
    }

    public class ApiRatingErrorDto
    {
        public string? Message { get; set; }
    }
}