using CookingAILand.Core.DAL.Enums;

namespace CookingAILand.Core.DAL.Entities;

public class Ingridient
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public MeasurementUnit Unit { get; set; }
    public double Amount { get; set; }
    public Guid recipeId { get; set; }
    public virtual Recipe? Recipe { get; set; }
}