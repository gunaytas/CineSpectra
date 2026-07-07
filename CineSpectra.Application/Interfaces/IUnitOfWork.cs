using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IShowRepository Shows { get; }

        Task<int> SaveChangesAsync();
    }
}