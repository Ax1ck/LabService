using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using NutritionCore.DTO;
using NutritionCore.Entities;
using NutritionCore.Services;

namespace NutritionCore.GraphQL;

public class MealMutations
{
    // Создать новый приём пищи
    public async Task<Meal> CreateMeal(
        MealDto input,
        [Service] IMealService mealService,
        CancellationToken cancellationToken)
    {
        var created = await mealService.Create(input, cancellationToken);
        return created;
    }

    // Обновить существующий приём пищи
    public async Task<Meal> UpdateMeal(
        Meal input,
        [Service] IMealService mealService)
    {
        await mealService.Update(input);
        // Можно вернуть обновлённую сущность
        return input;
    }

    // Удалить приём пищи
    public async Task<bool> DeleteMeal(
        string id,
        [Service] IMealService mealService)
    {
        await mealService.Delete(id);
        return true;
    }

    // Подтвердить приём пищи
    public async Task<Meal> ConfirmMeal(
        string id,
        Guid userId,
        DateTime confirmationTime,
        [Service] IMealService mealService)
    {
        await mealService.ConfirmMeal(id, userId, confirmationTime);
        return mealService.GetById(id);
    }
}