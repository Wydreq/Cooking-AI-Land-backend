namespace CookingAILand.Core.Models;

public class GetUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DateOfBirth { get; set; }
    public int RoleId { get; set; }
}