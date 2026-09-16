using CineSpectra.Application.DTOs;
using CineSpectra.WebMVC.Models;
using System.Net.Http.Json;
using RatingCriteriaDto = CineSpectra.Application.DTOs.RatingCriteriaDto;

namespace CineSpectra.WebMVC.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ShowDetailViewModel?> GetShowDetailsAsync(int showId)
    {
        // 1. Temel yapım bilgilerini API'den çek
        var showResponse = await _httpClient.GetAsync($"api/Shows/{showId}");
        if (!showResponse.IsSuccessStatusCode) return null;

        var showDto = await showResponse.Content.ReadFromJsonAsync<ShowDetailDto>();
        if (showDto == null) return null;

        // 2. Yapıma ait kriter ortalamalarını ve toplam oy istatistiğini çek
        var statsResponse = await _httpClient.GetAsync($"api/Ratings/show/{showId}");
        if (statsResponse.IsSuccessStatusCode)
        {
            showDto.RatingStats = await statsResponse.Content.ReadFromJsonAsync<ShowRatingStatsDto>();
        }

        // 3. Oylama formu için aktif kriter listesini çek
        var criteriaResponse = await _httpClient.GetAsync("api/Ratings/criteria");
        var activeCriteria = new List<RatingCriteriaDto>();
        if (criteriaResponse.IsSuccessStatusCode)
        {
            activeCriteria = await criteriaResponse.Content.ReadFromJsonAsync<List<RatingCriteriaDto>>() ?? new();
        }

        // 4. DTO'yu View'ın beklediği ViewModel'e aktar
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
            EpisodeAudienceScore = showDto.EpisodeAudienceScore, // Diziler için bölüm puanı
            Type = showDto.Type,
            ReleaseDate = showDto.ReleaseDate,
            Genres = showDto.Genres,
            Seasons = showDto.Seasons,
            Actors = showDto.Actors,
            Characters = showDto.Characters,
            RatingStats = showDto.RatingStats,
            ActiveCriteria = activeCriteria
        };

        return viewModel;
    }

    public async Task<bool> SubmitRatingAsync(object ratingDto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/Ratings", ratingDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<ActorDetailViewModel?> GetActorDetailsAsync(int actorId)
    {
        var response = await _httpClient.GetAsync($"api/actors/{actorId}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ActorDetailViewModel>();
    }

    public async Task<bool> SubmitActorRatingAsync(SubmitActorRatingViewModel ratingModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ratings/actor", ratingModel);
        return response.IsSuccessStatusCode;
    }

    public async Task<CharacterDetailViewModel?> GetCharacterDetailsAsync(int characterId)
    {
        var response = await _httpClient.GetAsync($"api/characters/{characterId}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<CharacterDetailViewModel>();
    }

    public async Task<List<CriteriaItemDto>> GetAllCriteriaAsync()
    {
        var response = await _httpClient.GetAsync("api/ratings/criteria");
        if (!response.IsSuccessStatusCode)
        {
            return new List<CriteriaItemDto>();
        }

        return await response.Content.ReadFromJsonAsync<List<CriteriaItemDto>>()
               ?? new List<CriteriaItemDto>();
    }

    public async Task<bool> SubmitFullRatingAsync(SubmitFullRatingInputModel input)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ratings/show-full", input);
        return response.IsSuccessStatusCode;
    }

    // SEZON OYLAMASI 
    public async Task<(bool IsSuccess, string Message)> SubmitSeasonRatingDirectAsync(int showId, int seasonId, double score)
    {
        try
        {
            var payload = new
            {
                showId = showId,
                seasonId = seasonId,
                score = score
            };

            var response = await _httpClient.PostAsJsonAsync("api/Ratings/season", payload);

            if (response.IsSuccessStatusCode)
            {
                return (true, "Sezon puanınız başarıyla kaydedildi.");
            }

            // API'den 400 Bad Request veya hata mesajı döndüyse içeriği oku
            var errorObj = await response.Content.ReadFromJsonAsync<ApiRatingErrorDto>();
            return (false, errorObj?.Message ?? "Sezon puanı işlenirken bir kural hatası oluştu.");
        }
        catch (Exception ex)
        {
            return (false, "Sunucuya bağlanılamadı: " + ex.Message);
        }
    }

    // BÖLÜM OYLAMASI 
    public async Task<(bool IsSuccess, string Message)> SubmitEpisodeRatingDirectAsync(int showId, int episodeId, double score)
    {
        try
        {
            var payload = new
            {
                showId = showId,
                episodeId = episodeId,
                score = score
            };

            var response = await _httpClient.PostAsJsonAsync("api/Ratings/episode", payload);

            if (response.IsSuccessStatusCode)
            {
                return (true, "Bölüm puanınız başarıyla kaydedildi.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<ApiRatingErrorDto>();
            return (false, errorObj?.Message ?? "Bölüm puanı işlenirken bir kural hatası oluştu.");
        }
        catch (Exception ex)
        {
            return (false, "Sunucuya bağlanılamadı: " + ex.Message);
        }
    }

    public class ApiRatingErrorDto
    {
        public string? Message { get; set; }
    }

}


