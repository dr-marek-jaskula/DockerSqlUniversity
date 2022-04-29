using Microsoft.EntityFrameworkCore;

namespace SakilaWebApi;

public class SakilaDbContext : DbContext
{
    public DbSet<Actor> Actors => Set<Actor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>().ToTable("actor");

        modelBuilder.Entity<Actor>().HasKey(a => a.Id);
        modelBuilder.Entity<Actor>().Property(a => a.Id).HasColumnName("actor_id");
        modelBuilder.Entity<Actor>().Property(a => a.FirstName).HasColumnName("first_name");
        modelBuilder.Entity<Actor>().Property(a => a.LastName).HasColumnName("last_name");
        modelBuilder.Entity<Actor>().Property(a => a.LastUpdate).HasColumnName("last_update");
    }

    public SakilaDbContext(DbContextOptions<SakilaDbContext> options) : base(options)
    {
    }
}