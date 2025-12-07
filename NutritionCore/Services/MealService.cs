using MongoDB.Driver;
using NutritionCore.Data;
using NutritionCore.DTO;
using NutritionCore.Entities;

namespace NutritionCore.Services;

public class MealService(MongoDbService mongoDbService, ProducerService producerService) : IMealService
{
    private readonly IMongoCollection<Meal> _meals = mongoDbService.Database.GetCollection<Meal>("meal");

    public async Task<IEnumerable<Meal>?> GetAll()
    {
        return await _meals.Find(FilterDefinition<Meal>.Empty).ToListAsync();
    }

    public Meal GetById(string id)
    {
        var filter = Builders<Meal>.Filter.Eq(x => x.Id, id);
        var meal = _meals.Find(filter).FirstOrDefault();
        return meal ?? throw new InvalidOperationException();
    }

    public async Task<Meal> Create(MealDto newMealDto, CancellationToken stoppingToken)
    {
        var meal = new Meal
        {
            UserId = newMealDto.UserId,
            Name = newMealDto.Name,
            Ingredients = newMealDto.Ingredients,
            Calories = newMealDto.Calories
        };
        await _meals.InsertOneAsync(meal, cancellationToken: stoppingToken);
        
        await producerService.ProduceAsync(stoppingToken,
            new ProducerMessage
            {
                ObjectId = meal.Id,
                UserId = meal.UserId
            });
        
        return meal;
    }

    public async Task Update(Meal newMeal)
    {
        var filter = Builders<Meal>.Filter.Eq(x => x.Id, newMeal.Id);
        var update = Builders<Meal>.Update
            .Set(x => x.Name, newMeal.Name)
            .Set(x => x.Ingredients, newMeal.Ingredients)
            .Set(x => x.Calories, newMeal.Calories);

        await _meals.UpdateOneAsync(filter, update);
    }

    public async Task Delete(string id)
    {
        var filter = Builders<Meal>.Filter.Eq(x => x.Id, id);
        var meal = _meals.Find(filter).FirstOrDefault();
        if(meal == null)
            throw new InvalidOperationException();
        await _meals.DeleteOneAsync(filter);
    }

    public async Task ConfirmMeal(string id, Guid userId, DateTime confirmationTime)
    {
        var filter = Builders<Meal>.Filter.Eq(news => news.Id, id);
        var update = Builders<Meal>.Update
            .Set(news => news.UserId, userId)
            .Set(news => news.ConfirmationTime, confirmationTime);

        await _meals.UpdateOneAsync(filter, update);
    }
}