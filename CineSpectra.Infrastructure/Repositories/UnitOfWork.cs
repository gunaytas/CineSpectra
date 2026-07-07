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
        private IShowRepository? _showRepository;

        public UnitOfWork(CineSpectraDbContext context)
        {
            _context = context;
        }

        public IShowRepository Shows => _showRepository ??= new ShowRepository(_context);

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