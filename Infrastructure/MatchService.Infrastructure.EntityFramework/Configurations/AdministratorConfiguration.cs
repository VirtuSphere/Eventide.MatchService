using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MatchService.Domain;
using MatchService.ValueObjects;
using MatchService.ValueObjects.Validators;

namespace MatchService.Infrastructure.EntityFramework.Configurations;

public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.ToTable("Administrators");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Username)
            .IsRequired()
            .HasConversion(username => username.Value, str => new Username(str))
            .HasMaxLength(UsernameValidator.MAX_LENGTH);

        builder.HasMany(a => a.Matches)
            .WithOne(m => m.Administrator)
            .HasForeignKey("AdministratorId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
