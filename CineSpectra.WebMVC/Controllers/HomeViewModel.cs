using System.Collections.Generic;
using CineSpectra.Application.DTOs;

namespace CineSpectra.WebMVC.Models
{
    public class HomeViewModel
    {
        public ShowDetailDto HeroShow { get; set; } = null!;

        public List<ShowListDto> PopularShows { get; set; } = new();

        public List<ShowListDto> RecentShows { get; set; } = new();

        public List<GenreGroupViewModel> GenreGroups { get; set; } = new();
    }

    public class GenreGroupViewModel
    {
        public string GenreName { get; set; } = string.Empty;
        public List<ShowListDto> Shows { get; set; } = new();
    }
}