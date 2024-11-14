using api.Extensions;
using api.Services;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddOptions();
builder.Services.ConfigureCors();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.ConfigureDependencyInjection();
builder.Services.ConfigureHangfire(builder.Configuration);

var app = builder.Build();

app.UseHangfireDashboard();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IPInfo v1"));
app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

RecurringJob.AddOrUpdate<IPUpdateService>(
        x => x.UpdateIPsAsync(),
        Cron.Hourly
);

app.Run();
