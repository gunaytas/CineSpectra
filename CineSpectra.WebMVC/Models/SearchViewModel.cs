using System.Collections.Generic;
using AppActorDto = CineSpectra.Application.DTOs.ActorDto;
using AppCharacterDto = CineSpectra.Application.DTOs.CharacterDto;
using CineSpectra.Application.DTOs;

namespace CineSpectra.WebMVC.Models
{
    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public string ActiveTab { get; set; } = "all"; // all, shows, actors, characters

        public List<ShowListDto> Shows { get; set; } = new();
        public List<AppActorDto> Actors { get; set; } = new();
        public List<AppCharacterDto> Characters { get; set; } = new();

        public int TotalCount => Shows.Count + Actors.Count + Characters.Count;
    }
}