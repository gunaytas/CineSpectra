using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
using CineSpectra.Infrastructure.Persistence;

namespace CineSpectra.Infrastructure.Repositories
{
    public class RatingCriteriaRepository : GenericRepository<RatingCriteria>, IRatingCriteriaRepository
    {
        public RatingCriteriaRepository(CineSpectraDbContext context) : base(context)
        {
        }
    }
}