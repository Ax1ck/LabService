using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutritionCore.DTO;
using NutritionCore.Entities;
using NutritionCore.Services;
using NutritionCore.Services.Caching;
using Prometheus;

namespace NutritionCore.Controllers;

[Route("[controller]")]
[ApiController]
public class MealsController(IMealService mealService, IRedisCacheService cache, ProducerService producer)
    : ControllerBase
{
    private static readonly Counter MealsCounter =
        Metrics.CreateCounter(
            "meals_total",
            "Total number of meals");

    private static readonly Counter MealsFailedCounter =
        Metrics.CreateCounter(
            "meals_Failed_total",
            "Total number of failed meals");
    
    private static readonly Histogram RegistrationDuration =
        Metrics.CreateHistogram(
            "post_duration_seconds",
            "duration POST in seconds");
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Meal>?>> GetAll()
    {
        try
        {
            var meals = await mealService.GetAll();

            return Ok(meals);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        try
        {
            var key = $"mealsById{id}";
            var meal = cache.GetData<Meal>(key);
            if (meal is not null)
                return Ok(meal);
            meal = mealService.GetById(id);
            cache.SetData(key, meal);

            return Ok(meal);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> Create(MealDto meal, CancellationToken token)
    {
        using (RegistrationDuration.NewTimer())
        {

            try
            {
                var createdMeal = await mealService.Create(meal, token);
                MealsCounter.Inc();
                return CreatedAtAction(nameof(GetById), new { id = createdMeal.Id }, createdMeal);
            }
            catch (Exception)
            {
                MealsFailedCounter.Inc();
                throw;
            }
        }
    }

    [HttpPut]
    //[Authorize]
    public async Task<IActionResult> Update(Meal meal)
    {
        try
        {
            await mealService.Update(meal);
            cache.SetData($"mealsById{meal.Id}", meal);
        }
        catch
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpDelete("{id}")]
    //[Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await mealService.Delete(id);
            cache.Delete<Meal>($"mealsById{id}");
        }
        catch
        {
            return NotFound();
        }

        return Ok();
    }
}