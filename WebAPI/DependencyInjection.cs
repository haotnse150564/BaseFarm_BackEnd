using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Services;
using Application.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Application.Services.Implement;

namespace WebAPI;
public static class DependencyInjection
{
    public static void AddWebAPIService(this IServiceCollection services, WebApplicationBuilder builder)
    {
        #region thêm sau này
        builder.Services.AddHttpClient();
        services.AddHttpClient<IWeatherServices, WeatherService>(client =>
        {
            client.BaseAddress = new Uri("http://api.openweathermap.org/"); // Thay đổi URL cơ sở
        });

        #endregion
        services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        //services.AddControllers().AddNewtonsoftJson(o =>
        //{
        //    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //});

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BaseFarm_BE", Version = "v1" });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Please enter a valid token",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            c.AddSecurityDefinition("Bearer", securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
        });
        services.AddHealthChecks();
        services.AddHttpContextAccessor();
        services.AddScoped<IClaimsServices, ClaimsServices>();
        //Adding Session
        services.AddDistributedMemoryCache(); //Adding cache in memory for session.
        services.AddSession(); //Adding session.
        //services.AddTransient<ISessionServices, SessionServices>();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                RoleClaimType = ClaimTypes.Role,
                ValidAudience = builder.Configuration["JWT:Audience"],
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding
                    .UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            };

            //SignalR
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // Chỉ áp dụng cho SignalR hubs
                    var path = context.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        services.AddAuthorization();
        //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        //{
        //    options.RequireHttpsMetadata = false;
        //    options.SaveToken = true;
        //    options.TokenValidationParameters = new TokenValidationParameters()
        //    {
        //        ValidAudience = builder.Configuration["Jwt:Audience"],
        //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //        ValidateIssuerSigningKey = true,
        //        ValidateLifetime = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        //    };
        //});
        services.AddFluentValidationClientsideAdapters();


        services.AddCors(options =>
        {
            options.AddPolicy(name: "_myAllowSpecificOrigins",
                              policy =>
                              {
                                  policy.AllowAnyOrigin()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                              });
        });


    }
}