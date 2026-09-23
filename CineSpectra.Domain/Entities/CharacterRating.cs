using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CineSpectra.Domain.Common;

namespace CineSpectra.Domain.Entities;

public class CharacterRating : BaseEntity
{
    public string UserId { get; set; } = string.Empty; 
    public int CharacterId { get; set; } 
    public Character Character { get; set; } = null!;

    public int? CriteriaId { get; set; } 
    public RatingCriteria? Criteria { get; set; } = null!;

    public int Score { get; set; }
    public string? Comment { get; set; } 
}