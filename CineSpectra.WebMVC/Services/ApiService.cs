using System.Net.Http.Json;
using CineSpectra.WebMVC.Models;

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
        // 1. Yapım bilgileri
        var showResponse = await _httpClient.GetAsync($"/api/Shows/{showId}");
        if (!showResponse.IsSuccessStatusCode) return null;

        var showDetails = await showResponse.Content.ReadFromJsonAsync<ShowDetailViewModel>();
        if (showDetails == null) return null;

        // 2. Aktif kriterler
        var criteriaResponse = await _httpClient.GetAsync("/api/Ratings/criteria");
        if (criteriaResponse.IsSuccessStatusCode)
        {
            var criteria = await criteriaResponse.Content.ReadFromJsonAsync<List<RatingCriteriaDto>>();
            showDetails.ActiveCriteria = criteria ?? new();
        }

        return showDetails;
    }

    public async Task<bool> SubmitRatingAsync(object ratingDto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/Ratings", ratingDto);
        return response.IsSuccessStatusCode;
    }
}