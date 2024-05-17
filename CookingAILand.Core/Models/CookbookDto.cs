using Microsoft.AspNetCore.Http;

namespace CookingAILand.Core.Models;

public class CookbookDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Published { get; set; } = false;
    
    public IFormFile? Photo { get; set; }
}