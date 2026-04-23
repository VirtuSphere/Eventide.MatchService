using Eventide.MatchService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eventide.MatchService.Infrastructure.Data;

public class MatchDbContext : DbContext
{
    public DbSet<Match> Matches => Set<Match>();

    public MatchDbContext(DbContextOptions<MatchDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(builder =>
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Status).HasConversion<string>().IsRequired();
            builder.HasIndex(m => m.TournamentId);
            builder.HasIndex(m => m.Player1Id);
            builder.HasIndex(m => m.Player2Id);
        });
    }
}