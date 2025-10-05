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
using WebAPI.Services;
using Application.Utils;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfractstructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IUnitOfWorks, UnitOfWorks>();
        services.AddScoped<JWTUtils>();
        services.AddScoped<CheckDate>();

        //mapping

        services.AddAutoMapper(typeof(ProductsMapping));
        services.AddAutoMapper(typeof(FeedbackMapping));
        services.AddAutoMapper(typeof(OrderMapping));
        services.AddAutoMapper(typeof(AccountProfileMapping));
        services.AddAutoMapper(typeof(ScheduleMapping));
        services.AddAutoMapper(typeof(IOTMapping));
        services.AddAutoMapper(typeof(AccountMapping));
        services.AddAutoMapper(typeof(CropMapping));
        services.AddAutoMapper(typeof(FarmActitvityMapping));
        services.AddAutoMapper(typeof(FarmMapping));

        #region Config Repository and Service
        //repository
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountProfileRepository, AccountProfileRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IDevicesRepository, DevicesRepository>();
        services.AddScoped<ICropRepository, CropRepository>();
        services.AddScoped<IFarmActivityRepository, FarmActivityRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IFarmRepository, FarmRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ICropRequirementRepository, CropRequirementtRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        //service
        services.AddScoped<IProductServices, ProductServices>();
        services.AddScoped<IFeedbackSevices, FeedbackServices>();
        services.AddScoped<IAccountServices, AccountServices>();
        services.AddScoped<IAccountProfileServices, AccountProfileServices>();
        services.AddScoped<IOrderServices, OrderServices>();
        services.AddScoped<IVnPayService, VnPayService>();
        services.AddScoped<IScheduleServices, ScheduleServices>();
        services.AddScoped<IDevicesServices, DevicesServices>();
        services.AddScoped<ICropServices, CropServices>();
        services.AddScoped<IFarmActivityServices, FarmActivityServices>();
        services.AddScoped<ICategoryServices, CategoryServices>();
        services.AddScoped<IFarmServices, FarmServices>();
        services.AddScoped<IBlynkService, BlynkService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ICartServices, CartServices>();

        //services.AddScoped<IOrderDetailServices, OrderDetailServices>();
        //services.AddScoped<IAccountProfileServices, AccountProfileServices>();
        #endregion

        services.AddSingleton<ICurrentTime, CurrentTime>();
        // Use local DB
        //services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("Default")));
        services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(config.GetConnectionString("Default")));

        services.AddAutoMapper(typeof(MapperConfigs).Assembly);
        //AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        //AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        return services;

    }
}
