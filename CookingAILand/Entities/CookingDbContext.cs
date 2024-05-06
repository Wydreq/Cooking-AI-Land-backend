using Microsoft.EntityFrameworkCore;

namespace CookingAILand.Entities;

public class CookingDbContext : DbContext
{
    private string _connectionString =
        "Server=localhost;Database=CookingAILand;User Id=sa;Password=zaq1@WSX;TrustServerCertificate=True";

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().Property(r => r.Name).IsRequired();

        modelBuilder.Entity<User>()
            .Property(r => r.FirstName)
            .IsRequired()
            .HasMaxLength(25);

        modelBuilder.Entity<User>()
            .Property(r => r.LastName)
            .IsRequired()
            .HasMaxLength(25);

        modelBuilder.Entity<User>().Property(r => r.DateOfBirth).IsRequired();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}