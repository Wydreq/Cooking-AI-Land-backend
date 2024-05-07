using CookingAILand.Core.DAL.Enums;

namespace CookingAILand.Core.DAL.Entities;

public class Ingridient
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public MeasurementUnit Unit { get; set; }
    public double Amount { get; set; }
}