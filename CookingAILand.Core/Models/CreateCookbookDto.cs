namespace CookingAILand.Core.Models;

public class CreateCookbookDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Published { get; set; } = false;
}