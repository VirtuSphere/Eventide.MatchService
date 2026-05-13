using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MatchService.Domain;

namespace MatchService.Infrastructure.EntityFramework.Configurations
{
    public class BracketConfiguration : IEntityTypeConfiguration<Bracket>
    {
        public void Configure(EntityTypeBuilder<Bracket> builder)
        {
            builder.ToTable("Brackets");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
        }
    }
}
