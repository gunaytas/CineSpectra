using CineSpectra.Application;
using CineSpectra.Infrastructure;
using CineSpectra.Infrastructure.Persistence;
using CineSpectra.Infrastructure.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<CineSpectraDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));


// .NET 9 Yerleþik OpenAPI Servisi
builder.Services.AddOpenApi();

// DataSeeder IoC Container 
builder.Services.AddScoped<DataSeeder>();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI paneli ayarlarý
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CineSpectra API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// --- DATA SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CineSpectraDbContext>();
        var seeder = services.GetRequiredService<DataSeeder>();

        await seeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Data seeding adýmý sýrasýnda beklenmedik bir hata meydana geldi!");
    }
}

app.MapControllers();
app.Run();