using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MatchService.Domain;
using MatchService.Domain.Enums;
using MatchService.ValueObjects;
using MatchService.ValueObjects.Validators;
using System;

namespace MatchService.Infrastructure.EntityFramework.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("Matches");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).IsRequired();

        builder.Property(m => m.Status).IsRequired().HasConversion<string>();

        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc));

        builder.Property(m => m.ScheduledTime)
            .HasConversion(
                src => !src.HasValue ? src : src.Value.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src.Value, DateTimeKind.Utc),
                dst => !dst.HasValue ? dst : dst.Value.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst.Value, DateTimeKind.Utc));

        builder.Property(m => m.StartedAt)
            .HasConversion(
                src => !src.HasValue ? src : src.Value.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src.Value, DateTimeKind.Utc),
                dst => !dst.HasValue ? dst : dst.Value.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst.Value, DateTimeKind.Utc));

        builder.Property(m => m.CompletedAt)
            .HasConversion(
                src => !src.HasValue ? src : src.Value.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src.Value, DateTimeKind.Utc),
                dst => !dst.HasValue ? dst : dst.Value.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst.Value, DateTimeKind.Utc));

        builder.Property(m => m.MapName)
            .IsRequired(false)
            .HasConversion(
                mapName => mapName != null ? mapName.Value : null,
                str => str != null ? new MapName(str) : null
            )
            .HasMaxLength(MapNameValidator.MAX_LENGTH);

        builder.Property(m => m.ServerInfo)
            .IsRequired(false)
            .HasConversion(
                serverInfo => serverInfo != null ? serverInfo.Value : null,
                str => str != null ? new ServerInfo(str) : null
            )
            .HasMaxLength(ServerInfoValidator.MAX_LENGTH);

        builder.Property(m => m.Player1Score)
            .IsRequired(false)
            .HasConversion(
                score => score != null ? score.Value : (int?)null,
                value => value.HasValue ? new PlayerScore(value.Value) : null
            );

        builder.Property(m => m.Player2Score)
            .IsRequired(false)
            .HasConversion(
                score => score != null ? score.Value : (int?)null,
                value => value.HasValue ? new PlayerScore(value.Value) : null
            );

        builder.HasOne(m => m.Tournament)
            .WithMany()
            .HasForeignKey("TournamentId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Bracket)
            .WithMany()
            .HasForeignKey("BracketId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Administrator)
            .WithMany(a => a.Matches)
            .HasForeignKey("AdministratorId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Player1)
            .WithMany()
            .HasForeignKey("Player1Id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Player2)
            .WithMany()
            .HasForeignKey("Player2Id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Winner)
            .WithMany()
            .HasForeignKey("WinnerId")
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
