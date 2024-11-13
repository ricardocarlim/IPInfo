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

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddOptions();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

#region hangfire services

builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();
#endregion

#region DI
builder.Services.AddScoped<IIPAddressRepository, IPAddressRepository>();
builder.Services.AddScoped<IIPAddressService, IPAddressService>();

builder.Services.AddScoped<IIPUpdateRepository, IPUpdateRepository>();
builder.Services.AddScoped<IIPUpdateService, IPUpdateService>();

builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryService, CountryService>();

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IP2CCacheService>();
#endregion

#region Configure Swagger  
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IPInfo", Version = "v1" });    
});
#endregion

#region Database configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);
#endregion

#region Redis Configuration
var redisConfig = builder.Configuration.GetSection("Redis");

var redisConnection = redisConfig["ConnectionString"];
var instanceName = redisConfig["InstanceName"];

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;  
    options.InstanceName = instanceName;      
});
#endregion

var app = builder.Build();

#region hangfire services
app.UseHangfireDashboard(); 
app.UseHangfireServer();

RecurringJob.AddOrUpdate<IPUpdateService>(
        x => x.UpdateIPsAsync(),
        Cron.Hourly
    );

#endregion

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "REC.Api.DailyMotion v1"));
app.UseCors(x => { x.AllowAnyHeader(); x.AllowAnyMethod(); x.AllowAnyOrigin(); });
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

