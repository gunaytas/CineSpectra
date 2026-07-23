using CineSpectra.Application.Interfaces;
using CineSpectra.Application.Services;
using CineSpectra.Infrastructure.Persistence;
using CineSpectra.Infrastructure.Repositories;
using CineSpectra.WebMVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<CineSpectraDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IShowRatingRepository, MediaRatingRepository>();
builder.Services.AddScoped<IShowRatingService, ShowRatingService>();
builder.Services.AddScoped<IRatingCriteriaRepository, RatingCriteriaRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IShowService, ShowService>();

builder.Services.AddAutoMapper(typeof(CineSpectra.Application.Mapping.MappingProfile));

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7101");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
