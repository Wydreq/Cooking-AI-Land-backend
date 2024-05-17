using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CookingAILand.Models;

public class RegisterUserDto
{
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] [MaxLength(25)] public string FirstName { get; set; }
    [Required] [MaxLength(25)] public string LastName { get; set; }
    [Required] public DateTime DateOfBirth { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string ConfirmPassword { get; set; }
    public int RoleId { get; set; } = 1;
}