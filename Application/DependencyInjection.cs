using Application;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Mapper;
using Infrastructure.Repositories.Implement;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Services.Implement;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfractstructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IUnitOfWorks, UnitOfWorks>();

        //repository
        services.AddScoped<IProductRepository, ProductRepository>();

        //service
        services.AddScoped<IProductServices, ProductServices>();

        //mapping
        services.AddAutoMapper(typeof(ProductsMapping));
        #region Config Repository and Service


        #endregion

        services.AddSingleton<ICurrentTime, CurrentTime>();
        // Use local DB
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("Default")));
        services.AddAutoMapper(typeof(MapperConfigs).Assembly);

        return services;

    }
}
