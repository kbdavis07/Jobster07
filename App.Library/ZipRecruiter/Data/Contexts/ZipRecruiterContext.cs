using App.Library.ZipRecruiter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Library.ZipRecruiter.Data.Contexts;

public class ZipRecruiterContext : DbContext
{
    private readonly DbContextOptions<ZipRecruiterContext> _options;
    public DbContextOptions<ZipRecruiterContext> Options
    {
        get
        {
            return _options;
        }
    }

    public ZipRecruiterContext(DbContextOptions<ZipRecruiterContext> options) : base(options)
    {
        _options = options;
    }


    public DbSet<Job> Jobs { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>()
            .HasKey(j => j.Id);

        modelBuilder.Entity<Company>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Location>()
            .HasKey(l => l.Text);

    }
}
