namespace CookingAILand.Core.DAL.Entities;

public class Cookbook
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Published { get; set; } = false;
    public List<Recipe>? Recipes { get; set; }
    public string? PhotoUrl { get; set; }
    public Guid CreatedById { get; set; }
    public virtual User CreatedBy { get; set; }
}