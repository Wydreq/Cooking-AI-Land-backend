using CookingAILand.Core.Entities;

namespace CookingAILand.Core.DAL.Entities;

public class Recipe
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Instructions { get; set; }
    public int Servings { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public List<Ingridient> Ingridients { get; set; }
    public int CookbookId { get; set; }
    public virtual Cookbook? Cookbook { get; set; }
    public int? CreatedById { get; set; }
    public virtual User CreatedBy { get; set; }
}