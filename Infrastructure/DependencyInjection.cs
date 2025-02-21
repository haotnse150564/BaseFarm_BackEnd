using Application;
using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfractstructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IUnitOfWorks, UnitOfWorks>();

        #region Config Repository and Service
       

        #endregion

        //services.AddSingleton<ICurrentTime, CurrentTime>();
        //// Use local DB
        //services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("Default")));
        //services.AddAutoMapper(typeof(MapperConfigs).Assembly);

        return services;

    }
}
