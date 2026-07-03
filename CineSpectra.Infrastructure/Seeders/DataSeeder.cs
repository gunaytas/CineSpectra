using CineSpectra.Domain.Entities;
using CineSpectra.Domain.Enums;
using CineSpectra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TMDbLib.Client;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
using TMDbLib.Objects.People;

namespace CineSpectra.Infrastructure.Seeders
{
    public static class DataSeeder
    {
        private const string TmdbApiKey = "a09b4cc016465e07aceb2fb6b899ea72";

        public static async Task SeedAsync(CineSpectraDbContext context)
        {
            await context.Database.MigrateAsync();

            // Dinamik Oylama Kriterleri
            if (!await context.RatingCriterias.AnyAsync())
            {
                var criterias = new List<RatingCriteria>
                {
                    new() { Name = "Görüntü Yönetmenliği (Cinematography)", Target = CriteriaTarget.Show },
                    new() { Name = "Dekor ve Set Tasarımı", Target = CriteriaTarget.Show },
                    new() { Name = "Kostüm Tasarımı", Target = CriteriaTarget.Show },
                    new() { Name = "Senaryo ve Hikaye Akışı", Target = CriteriaTarget.Show },
                    new() { Name = "Karakter Gelişimi (Senaryo)", Target = CriteriaTarget.Character },
                    new() { Name = "Mimik ve Beden Dili Kullanımı", Target = CriteriaTarget.Actor },
                    new() { Name = "Diksiyon ve Hitabet Performansı", Target = CriteriaTarget.Actor }
                };

                await context.RatingCriterias.AddRangeAsync(criterias);
                await context.SaveChangesAsync();
            }
            
            // Veritabanı oluşturma
            if (!await context.Shows.AnyAsync())
            {
                var people = new Person();

                var client = new TMDbClient(TmdbApiKey);

                var popularTvShows = await client.GetTvShowPopularAsync(language: "tr-TR");

                if (popularTvShows?.Results != null)
                {
                    foreach (var searchTv in popularTvShows.Results.Take(5))
                    {
                        TvShow? tmdbShow = await client.GetTvShowAsync(searchTv.Id, TvShowMethods.Credits);

                        if (tmdbShow == null) continue;
                        
                        var showGenres = new List<Genre>();

                        if (tmdbShow.Genres != null)
                        {
                            foreach (var tmdbGenre in tmdbShow.Genres)
                            {
                                var existingGenre = await context.Genres.FirstOrDefaultAsync(g => g.Id == tmdbGenre.Id);
                                
                                if (existingGenre == null)
                                {
                                    existingGenre = new Genre { Id = tmdbGenre.Id, Name = tmdbGenre.Name ?? "Bilinmeyen Tür" };
                                    await context.Genres.AddAsync(existingGenre);
                                    await context.SaveChangesAsync();
                                }
                                showGenres.Add(existingGenre);
                            }
                        }

                        var directorName = tmdbShow.CreatedBy?.FirstOrDefault()?.Name
                            ?? tmdbShow.Credits?.Crew?.FirstOrDefault(c => c.Job == "Director")?.Name
                            ?? tmdbShow.Credits?.Crew?.FirstOrDefault(c => c.Job == "Executive Producer")?.Name
                            ?? "Bilinmeyen Yönetmen";
                        var companyName = tmdbShow.ProductionCompanies?.FirstOrDefault()?.Name ?? "Bilinmeyen Yapım Şirketi";

                        
                        var newShow = new Show
                        {
                            Title = tmdbShow.Name ?? "Adsız Yapım",
                            Description = tmdbShow.Overview ?? "Açıklama bulunamadı.",
                            CoverImageUrl = string.IsNullOrEmpty(tmdbShow.PosterPath)
                                ? "https://via.placeholder.com/500x750?text=No+Image"
                                : $"https://image.tmdb.org/t/p/w500{tmdbShow.PosterPath}",
                            Type = MediaType.TVShow,
                            SeasonsCount = tmdbShow.NumberOfSeasons,
                            EpisodesCount = tmdbShow.NumberOfEpisodes,
                            Director = directorName,
                            ProductionCompany = companyName,
                            AverageScore = 0.0,
                            Genres = showGenres
                        };

                        await context.Shows.AddAsync(newShow);
                        await context.SaveChangesAsync();

                        if (tmdbShow.Credits?.Cast != null)
                        {
                            foreach (var castMember in tmdbShow.Credits.Cast)
                            {
                                if (string.IsNullOrWhiteSpace(castMember.Name) || string.IsNullOrWhiteSpace(castMember.Character))
                                    continue;
                                
                                var existingActor = await context.Actors.FirstOrDefaultAsync(a => a.Name == castMember.Name)
                                                    ?? context.Actors.Local.FirstOrDefault(a => a.Name == castMember.Name);


                                if (existingActor == null)
                                {
                                    var personDetails = await client.GetPersonAsync(castMember.Id);

                                    existingActor = new Actor
                                    {
                                        Name = castMember.Name,
                                        BirthDate = personDetails?.Birthday,
                                        Biography = personDetails?.Biography ?? "Biyografi bulunamadı.",
                                        ProfileImageUrl = string.IsNullOrEmpty(castMember.ProfilePath)
                                            ? "https://via.placeholder.com/300x450?text=No+Image"
                                            : $"https://image.tmdb.org/t/p/w500{castMember.ProfilePath}",
                                        AverageScore = 0.0
                                    };
                                    await context.Actors.AddAsync(existingActor);
                                    await context.SaveChangesAsync();
                                }

                                
                                var existingCharacter = await context.Characters
                                    .Include(c => c.Shows)
                                    .FirstOrDefaultAsync(c => c.Name == castMember.Character && c.ActorId == existingActor.Id)
                                    ?? context.Characters.Local
                                    .FirstOrDefault(c => c.Name == castMember.Character && c.ActorId == existingActor.Id);

                                if (existingCharacter == null)
                                {
                                    var newCharacter = new Character
                                    {
                                        Name = castMember.Character,
                                        ImageUrl = string.IsNullOrEmpty(castMember.ProfilePath)
                                            ? "https://via.placeholder.com/500x750?text=No+Profile"
                                            : $"https://image.tmdb.org/t/p/w500{castMember.ProfilePath}",
                                        ActorId = existingActor.Id,
                                        AverageScore = 0.0
                                    };

                                    newCharacter.Shows.Add(newShow);
                                    await context.Characters.AddAsync(newCharacter);
                                }
                                else
                                {
                                    if (!existingCharacter.Shows.Any(s => s.Id == newShow.Id))
                                    {
                                        existingCharacter.Shows.Add(newShow);
                                    }
                                }

                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }
    }
}