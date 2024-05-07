using System.ComponentModel.DataAnnotations;

namespace CookingAILand.Core.Models;

public class ForgotPasswordDto
{
    [Required, EmailAddress]
    public string Email { get; set; }
    public bool EmailSent { get; set; }
}