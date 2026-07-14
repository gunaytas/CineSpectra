using CineSpectra.Application.Interfaces;
using CineSpectra.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CineSpectraDbContext _context;
        public IShowRepository Shows { get; private set; }
        public IRatingCriteriaRepository RatingCriterias { get; private set; }
        public IShowRatingRepository MediaRatings { get; private set; } 

        public UnitOfWork(
            CineSpectraDbContext context,
            IShowRepository shows,
            IRatingCriteriaRepository ratingCriterias,
            IShowRatingRepository mediaRatings) 
        {
            _context = context;
            Shows = shows;
            RatingCriterias = ratingCriterias;
            MediaRatings = mediaRatings; 
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}