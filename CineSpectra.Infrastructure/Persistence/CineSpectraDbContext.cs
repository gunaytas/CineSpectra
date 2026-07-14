using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CineSpectra.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineSpectra.Infrastructure.Persistence;

public class CineSpectraDbContext : DbContext
{
    public CineSpectraDbContext(DbContextOptions<CineSpectraDbContext> options) : base(options)
    {
    }

    public DbSet<Show> Shows => Set<Show>();
    public DbSet<Season> Seasons => Set<Season>(); 
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<RatingCriteria> RatingCriterias => Set<RatingCriteria>();
    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<MediaRating> MediaRatings => Set<MediaRating>();
    public DbSet<MediaRatingSubValue> MediaRatingSubValues { get; set; }
    public DbSet<CharacterRating> CharacterRatings => Set<CharacterRating>();
    public DbSet<ActorRating> ActorRatings => Set<ActorRating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Show>(entity => {
            entity.ToTable("Shows");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).HasMaxLength(255).IsRequired();
            entity.Property(s => s.CoverImageUrl).HasMaxLength(500);
            entity.Property(s => s.Type).IsRequired();
            entity.Property(s => s.AverageScore).HasDefaultValue(0.0);
        });

        modelBuilder.Entity<Season>(entity => {
            entity.ToTable("Seasons");
            entity.HasKey(s => s.Id);

            entity.HasOne(s => s.Show)
                  .WithMany(sh => sh.Seasons)
                  .HasForeignKey(s => s.ShowId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Episode>(entity => {
            entity.ToTable("Episodes");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Season)
                  .WithMany(s => s.Episodes)
                  .HasForeignKey(e => e.SeasonId)
                  .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Actor>(entity =>
        {
            entity.ToTable("Actors");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).HasMaxLength(255).IsRequired();
            entity.Property(a => a.AverageScore).HasDefaultValue(0.0);
            entity.Property(a => a.BirthDate).IsRequired(false);

            entity.HasMany(c => c.Shows)
                  .WithMany(s => s.Actors)
                  .UsingEntity<Dictionary<string, object>>(
                      "ShowActors",
                      j => j.HasOne<Show>().WithMany().HasForeignKey("ShowId").OnDelete(DeleteBehavior.Cascade),
                      j => j.HasOne<Actor>().WithMany().HasForeignKey("ActorId").OnDelete(DeleteBehavior.Cascade)
                  );
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.ToTable("Characters");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(255).IsRequired();
            entity.Property(c => c.ImageUrl).HasMaxLength(500);
            entity.Property(c => c.AverageScore).HasDefaultValue(0.0);

            entity.HasOne(c => c.Actor)
                  .WithMany(a => a.Characters)
                  .HasForeignKey(c => c.ActorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Shows)
                  .WithMany(s => s.Characters)
                  .UsingEntity<Dictionary<string, object>>(
                      "ShowCharacters", 
                      j => j.HasOne<Show>().WithMany().HasForeignKey("ShowId").OnDelete(DeleteBehavior.Cascade),
                      j => j.HasOne<Character>().WithMany().HasForeignKey("CharacterId").OnDelete(DeleteBehavior.Cascade)
                  );
        });

        modelBuilder.Entity<RatingCriteria>(entity =>
        {
            entity.ToTable("RatingCriterias");
            entity.HasKey(rc => rc.Id);
            entity.Property(rc => rc.Name).HasMaxLength(100).IsRequired();
            entity.Property(rc => rc.Target).IsRequired();
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genres");
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).HasMaxLength(100).IsRequired();

            entity.HasMany(g => g.Shows)
                  .WithMany(s => s.Genres)
                  .UsingEntity<Dictionary<string, object>>(
                      "ShowGenres",
                      j => j.HasOne<Show>().WithMany().HasForeignKey("ShowId"),
                      j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId")
                  );
        });


        modelBuilder.Entity<MediaRating>(entity => {
            entity.ToTable("MediaRatings");
            entity.HasKey(r => r.Id);

            entity.HasOne(r => r.Show)
                  .WithMany()
                  .HasForeignKey(r => r.ShowId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Season)
                  .WithMany()
                  .HasForeignKey(r => r.SeasonId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Episode)
                  .WithMany()
                  .HasForeignKey(r => r.EpisodeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MediaRatingSubValue>(entity =>
        {
            entity.ToTable("MediaRatingSubValues");
            entity.HasKey(sv => sv.Id);

            entity.HasOne(sv => sv.MediaRating)
                  .WithMany(r => r.SubValues)
                  .HasForeignKey(sv => sv.MediaRatingId)
                  .OnDelete(DeleteBehavior.Cascade); 

            entity.HasOne(sv => sv.Criteria)
                  .WithMany()
                  .HasForeignKey(sv => sv.CriteriaId)
                  .OnDelete(DeleteBehavior.Restrict); 
        });

        modelBuilder.Entity<CharacterRating>(entity =>
        {
            entity.ToTable("CharacterRatings");
            entity.HasKey(cr => cr.Id);
            entity.Property(cr => cr.UserId).HasMaxLength(255).IsRequired();
            entity.Property(cr => cr.Score).IsRequired();

            entity.HasOne(cr => cr.Character)
                  .WithMany(c => c.Ratings)
                  .HasForeignKey(cr => cr.CharacterId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cr => cr.Criteria)
                  .WithMany()
                  .HasForeignKey(cr => cr.CriteriaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ActorRating>(entity =>
        {
            entity.ToTable("ActorRatings");
            entity.HasKey(ar => ar.Id);
            entity.Property(ar => ar.UserId).HasMaxLength(255).IsRequired();
            entity.Property(ar => ar.Score).IsRequired();

            entity.HasOne(ar => ar.Actor)
                  .WithMany(a => a.Ratings)
                  .HasForeignKey(ar => ar.ActorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ar => ar.Criteria)
                  .WithMany()
                  .HasForeignKey(ar => ar.CriteriaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        
    }
}