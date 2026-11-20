using Microsoft.EntityFrameworkCore;
using MatchService.Domain;

namespace MatchService.Infrastructure.EntityFramework;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Administrator> Administrators { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<Bracket> Brackets { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Sensitive data logging disabled for security
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }


}
