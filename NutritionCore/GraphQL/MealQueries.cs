using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using NutritionCore.Entities;
using NutritionCore.Services;

namespace NutritionCore.GraphQL;

public class MealQueries
{
    // Получить все приёмы пищи
    public Task<IEnumerable<Meal>> Meals(
        [Service] IMealService mealService)
        => mealService.GetAll();

    // Получить один приём пищи по Id
    public Meal MealById(
        string id,
        [Service] IMealService mealService)
        => mealService.GetById(id);

    // Пример чуть более «умного» запроса — фильтрация по пользователю
    public async Task<IEnumerable<Meal>> MealsByUserId(
        Guid userId,
        [Service] IMealService mealService)
    {
        var allMeals = await mealService.GetAll();
        return allMeals.Where(m => m.UserId == userId);
    }
}