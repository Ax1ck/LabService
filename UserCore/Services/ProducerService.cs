using Confluent.Kafka;
using UserCore.DTOs;
using UserCore.Json;

namespace UserCore.Services;

public class ProducerService
{
    private const string Topic = "MealConfirmation";

    public async Task ProduceAsync(CancellationToken stoppingToken, ProducerMessage message)
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
        await producer.ProduceAsync(topic: Topic,
            new Message<Null, ProducerMessage>()
            {
                Value = message
            }, stoppingToken);

        producer.Flush(stoppingToken);
    }
}