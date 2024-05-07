using CookingAILand.Core.Entities;

namespace CookingAILand.Core.DAL.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    
    public DateTime Created { get; set; } = DateTime.Now;
    public string PasswordHash { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
}