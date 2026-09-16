using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CineSpectra.Application.DTOs;
using CineSpectra.Application.Interfaces;

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

        public async Task<ShowDetailDto?> GetShowDetailAsync(int id)
        {
            var show = await _unitOfWork.Shows.GetShowDetailWithCastAndGenresAsync(id);

            if (show == null) return null;

            var showDto = _mapper.Map<ShowDetailDto>(show);

            showDto.RatingStats = await _ratingService.GetShowRatingStatsAsync(id);

            return showDto;
        }
    }
}