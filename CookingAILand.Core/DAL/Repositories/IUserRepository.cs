using CookingAILand.Core.DAL.Entities;

namespace CookingAILand.Core.DAL.Repositories;

public interface IUserRepository
{
    Task AddAsync(User? user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
}