using CookingAILand.Core.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CookingAILand.Core.DAL.Persistence;

public class CookingDbContext : DbContext
{
    private string _connectionString =
        "Server=localhost;Database=CookingAILand.Api;User Id=sa;Password=zaq1@WSX;TrustServerCertificate=True";

    public DbSet<User> Users { get; set; }
    public DbSet<Cookbook> Cookbooks { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingridient> Ingridients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}