using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.Entities;

namespace CookingAILand.Core.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CookingDbContext _dbContext;

    public UserRepository(CookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(User? user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users.FindAsync(email);
    }

    public async Task GenerateForgotPasswordTokenAsync()
    {
        
    }
}