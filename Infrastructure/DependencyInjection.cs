using Eventide.MatchService.Domain.Interfaces;
using Eventide.MatchService.Infrastructure.Data;
using Eventide.MatchService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eventide.MatchService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<MatchDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("MatchDb")));

        services.AddScoped<IMatchRepository, MatchRepository>();
        return services;
    }
}