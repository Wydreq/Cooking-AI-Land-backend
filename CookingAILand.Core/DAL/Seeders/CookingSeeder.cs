using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CookingAILand;

public class CookingSeeder
{
    private readonly CookingDbContext _dbContext;

    public CookingSeeder(CookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        if (_dbContext.Database.CanConnect())
        {
            var pendingMigrations = _dbContext.Database.GetPendingMigrations();
            if (pendingMigrations != null && pendingMigrations.Any())
            {
                _dbContext.Database.Migrate();
            }

            // if (!_dbContext.Roles.Any())
            // {
            //     var roles = GetRoles();
            //     _dbContext.Roles.AddRange(roles);
            //     _dbContext.SaveChanges();
            // }
        }
    }
}
    