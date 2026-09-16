using CineSpectra.Application.Interfaces;
using CineSpectra.Domain.Entities;
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
        private IGenericRepository<Season>? _seasons;
        public IGenericRepository<Season> Seasons => _seasons ??= new GenericRepository<Season>(_context);

        private IGenericRepository<Episode>? _episodes;
        public IGenericRepository<Episode> Episodes => _episodes ??= new GenericRepository<Episode>(_context);

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