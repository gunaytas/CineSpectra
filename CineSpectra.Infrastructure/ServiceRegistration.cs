using CineSpectra.Application.Interfaces;
using CineSpectra.Infrastructure.Persistence;
using CineSpectra.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CineSpectra.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CineSpectraDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IShowRepository, ShowRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}