using Confluent.Kafka;
using NutritionCore.DTO;
using NutritionCore.Json;

namespace NutritionCore.Services;

public class ProducerService(ILogger<ProducerService> logger)
{
    private const string Topic = "MealRegistarion";
    
    public async Task ProduceAsync(CancellationToken cancellationToken, ProducerMessage message)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "kafka:9092",
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };

        using var producer = new ProducerBuilder<Null, ProducerMessage>(config)
            .SetValueSerializer(new JsonSerializer<ProducerMessage>())
            .Build();

        try
        {
            var deliveryResult = await producer.ProduceAsync(topic: Topic,
                new Message<Null, ProducerMessage>
                {
                    Value = message
                },
                cancellationToken);
            
            Console.WriteLine($"userId = {message.UserId}, objectId = {message.ObjectId}");
        }
        catch (ProduceException<Null, string> ex)
        {
            logger.LogError($"Неудачная доставка: {ex.Error.Reason}");
        }

        producer.Flush(cancellationToken);
    }
}