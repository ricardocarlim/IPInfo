using api.Domain.Repositories;
using api.Domain.Services;
using api.Infraestructure;
using api.Persistence.Contexts;
using api.Persistence.Repositories;
using api.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IPInfo", Version = "v1" });
            });
        }

        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );
        }

        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("Redis");
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig["ConnectionString"]));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig["ConnectionString"];
                options.InstanceName = redisConfig["InstanceName"];
            });
        }

        public static void ConfigureDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<Domain.Repositories.ITransaction, Transaction>();

            services.AddScoped<IIPAddressRepository, IPAddressRepository>();
            services.AddScoped<IIPUpdateRepository, IPUpdateRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            
            services.AddScoped<IIPAddressService, IPAddressService>();
            services.AddScoped<IIPUpdateService, IPUpdateService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IReportService, ReportService>();

        
            services.AddSingleton<IP2CCacheService>();
        }

        public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            services.AddHangfireServer();
        }
    }
}
