using System.Threading.Tasks;
using CineSpectra.Application.DTOs;

namespace CineSpectra.Application.Interfaces
{
    public interface ICharacterRatingService
    {
        Task<RatingResultDto> SubmitCharacterRatingAsync(CreateCharacterRatingDto dto);
    }
}