using CookingAILand.Core.DAL.Entities;

namespace CookingAILand.Core.Models;

public class GetCookbookDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Published { get; set; } = false;
    public List<Recipe>? Recipes { get; set; }
}