using AutoMapper;
using CineSpectra.Application;
using CineSpectra.Domain.Entities;
using CineSpectra.Infrastructure;
using CineSpectra.Infrastructure.Persistence;
using CineSpectra.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddIdentity<AppUser, AppRole>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 6;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<CineSpectraDbContext>()
.AddDefaultTokenProviders();

var jwtSecretKey = builder.Configuration["Jwt:Key"] ?? "CineSpectra_Super_Secret_Key_For_Jwt_Auth_2026";
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

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