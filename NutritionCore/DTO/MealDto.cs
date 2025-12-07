namespace NutritionCore.DTO;

public class MealDto
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Ингредиенты.
    /// </summary>
    public string Ingredients { get; set; }
    /// <summary>
    /// Калории.
    /// </summary>
    public string Calories { get; set; }
    public Guid UserId { get; set; }
}