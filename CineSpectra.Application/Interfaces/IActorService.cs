using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineSpectra.Application.DTOs; 

namespace CineSpectra.Application.Interfaces
{
    public interface IActorService
    {
        Task<ActorDetailDto?> GetActorDetailAsync(int actorId);
    }
}