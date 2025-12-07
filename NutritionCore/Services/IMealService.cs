using NutritionCore.DTO;
using NutritionCore.Entities;

namespace NutritionCore.Services;

public interface IMealService
{
    Task<IEnumerable<Meal>?> GetAll();
    Meal GetById(string id);
    Task<Meal> Create(MealDto meal, CancellationToken token);
    Task Update(Meal newMeal);
    Task Delete(string id);
    Task ConfirmMeal(string id, Guid userId, DateTime confirmationTime);
}