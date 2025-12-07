using Confluent.Kafka;
using MongoDB.Bson;
using NutritionCore.DTO;
using NutritionCore.Json;

namespace NutritionCore.Services;

public class ConsumerService(IServiceProvider serviceProvider) : BackgroundService
{
    private const string Topic = "MealConfirmation";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "MealGroup",
            BootstrapServers = "kafka:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        Task.Run(async () =>
        {
            using (var consumer = new ConsumerBuilder<Ignore, ConsumerResult>(config)
                       .SetValueDeserializer(new JsonDeserializer<ConsumerResult>())
                       .Build())
            {
                consumer.Subscribe(Topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumerResult = consumer.Consume(TimeSpan.FromSeconds(5));
                    
                    if (consumerResult is null)
                        continue;

                    var result = consumerResult.Message.Value;

                    Console.WriteLine(
                        $"Получили сообщение: Date = {result.ConfirmationTime}, confirmed = {result.Confirmed}," +
                        $" userId = {result.UserId}, objectId = {result.ObjectId}");
                    
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var mealService = scope.ServiceProvider.GetRequiredService<IMealService>();

                        if (result.Confirmed)
                            await mealService.ConfirmMeal(result.ObjectId, result.UserId, result.ConfirmationTime);
                    }

                    consumer.Commit(consumerResult);
                }
            }
        }, stoppingToken);
        
        return Task.CompletedTask;
    }
}
