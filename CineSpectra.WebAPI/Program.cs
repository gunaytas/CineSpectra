using CineSpectra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- VERÝTABANI BAÐLANTISI ---
builder.Services.AddDbContext<CineSpectraDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// .NET 9 Yerleþik OpenAPI Servisini Ekle
builder.Services.AddOpenApi();

var app = builder.Build();

// --- PIPELINE (ARA YAZILIM) HATTI ---
if (app.Environment.IsDevelopment())
{
    // .NET 9 OpenAPI endpoint'ini aktif eder (/openapi/v1.json)
    app.MapOpenApi();

    // Bu JSON þemasýný alýp görsel Swagger paneline dönüþtüren arayüzcü:
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CineSpectra API v1");
        options.RoutePrefix = "swagger"; // Tarayýcýda hangi adrese gideceðini söyler
    });
}

app.UseHttpsRedirection();

// --- ÖRNEK ENDPOINT ---
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Uygulama tam ayaða kalkarken otomatik Data Seeding mekanizmasý
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CineSpectraDbContext>();

        await CineSpectra.Infrastructure.Seeders.DataSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Data seeding adýmý sýrasýnda beklenmedik bir hata meydana geldi!");
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}