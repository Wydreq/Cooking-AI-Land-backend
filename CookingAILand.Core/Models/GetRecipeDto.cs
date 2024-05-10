using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.Entities;

namespace CookingAILand.Core.Models;

public class GetRecipeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Instructions { get; set; }
    public int Servings { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public List<Ingridient> Ingridients { get; set; }
}