using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MatchService.Domain;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MatchService.Infrastructure.EntityFramework;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<Match> Matchs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }


}
