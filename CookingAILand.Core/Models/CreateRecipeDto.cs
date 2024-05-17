using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace CookingAILand.Core.Models;

public class CreateRecipeDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Instructions { get; set; }
    public int Servings { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public List<Ingridient> Ingridients { get; set; }
    public IFormFile? Photo { get; set; }
}