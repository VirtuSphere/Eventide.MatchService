using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MatchService.Domain;
using MatchService.Domain.Enums;
using MatchService.ValueObjects;
using MatchService.ValueObjects.Validators;

namespace MatchService.Infrastructure.EntityFramework.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("Matches");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.TournamentId).IsRequired();
        builder.Property(m => m.BracketId).IsRequired();
        builder.Property(m => m.Status).IsRequired().HasConversion<string>();
        builder.Property(m => m.CreatedAt).IsRequired();
        builder.Property(m => m.ScheduledTime);
        builder.Property(m => m.StartedAt);
        builder.Property(m => m.CompletedAt);

        // Value Objects
        builder.Property(m => m.MapName)
            .HasConversion(
                mapName => mapName != null ? mapName.Value : null,
                str => str != null ? new MapName(str) : null
            )
            .HasMaxLength(MapNameValidator.MAX_LENGTH);

        builder.Property(m => m.ServerInfo)
            .HasConversion(
                serverInfo => serverInfo != null ? serverInfo.Value : null,
                str => str != null ? new ServerInfo(str) : null
            )
            .HasMaxLength(ServerInfoValidator.MAX_LENGTH);

        builder.Property(m => m.Player1Score)
            .HasConversion(
                score => score != null ? score.Value : (int?)null,
                value => value.HasValue ? new PlayerScore(value.Value) : null
            );

        builder.Property(m => m.Player2Score)
            .HasConversion(
                score => score != null ? score.Value : (int?)null,
                value => value.HasValue ? new PlayerScore(value.Value) : null
            );

        // Relationships
        builder.HasOne<Administrator>(m => m.Administrator)
            .WithMany(a => a.Matches)
            .HasForeignKey("AdministratorId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>(m => m.Player1)
            .WithMany()
            .HasForeignKey("Player1Id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>(m => m.Player2)
            .WithMany()
            .HasForeignKey("Player2Id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>(m => m.Winner)
            .WithMany()
            .HasForeignKey("WinnerId")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
