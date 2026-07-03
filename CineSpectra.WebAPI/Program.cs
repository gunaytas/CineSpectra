using CineSpectra.Infrastructure.Persistence;
using CineSpectra.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<CineSpectraDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// .NET 9 Yerleþik OpenAPI Servisi
builder.Services.AddOpenApi();

// DataSeeder IoC Container 
builder.Services.AddScoped<DataSeeder>();

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

// --- UYGULAMA BAÞLARKEN OTOMATÝK DATA SEEDING ---
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

app.Run();