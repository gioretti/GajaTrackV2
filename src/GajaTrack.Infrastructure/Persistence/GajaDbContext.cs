using GajaTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Persistence;

public class GajaDbContext(DbContextOptions<GajaDbContext> options) : DbContext(options)
{
    public DbSet<NursingFeed> NursingFeeds { get; set; }
    public DbSet<BottleFeed> BottleFeeds { get; set; }
    public DbSet<SleepSession> SleepSessions { get; set; }
    public DbSet<DiaperChange> DiaperChanges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // NursingFeed Configuration
        modelBuilder.Entity<NursingFeed>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId).IsUnique();
            entity.Property(e => e.ExternalId).IsRequired();
        });

        // BottleFeed Configuration
        modelBuilder.Entity<BottleFeed>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId).IsUnique();
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Content).IsRequired();
        });

        // SleepSession Configuration
        modelBuilder.Entity<SleepSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId).IsUnique();
            entity.Property(e => e.ExternalId).IsRequired();
        });

        // DiaperChange Configuration
        modelBuilder.Entity<DiaperChange>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExternalId).IsUnique();
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Type).IsRequired();
        });
    }
}
