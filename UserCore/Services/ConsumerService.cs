using Confluent.Kafka;
using UserCore.DTOs;
using UserCore.Interfaces.Services;
using UserCore.Json;

namespace UserCore.Services;

public class ConsumerService(
    ILogger<ConsumerConfig> logger,
    IServiceProvider serviceProvider,
    ProducerService producerService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    var config = new ConsumerConfig
    {
        BootstrapServers = "kafka:9092",
        GroupId = "UserGroup",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    using var consumer = new ConsumerBuilder<Ignore, ConsumerResult>(config)
        .SetValueDeserializer(new JsonDeserializer<ConsumerResult>())
        .Build();

    consumer.Subscribe("MealRegistarion");

    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            var consumerResult = consumer.Consume(TimeSpan.FromSeconds(5));
            if (consumerResult == null) 
                continue;

            var result = consumerResult.Message.Value;

            using (var scope = serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var message = new ProducerMessage();

                if (await userService.AddRegisteredObject(result.UserId))
                {
                    message.Confirmed = true;
                    message.UserId = result.UserId;
                    message.ObjectId = result.ObjectId;
                    message.ConfirmationTime = DateTime.Now;
                }

                consumer.Commit(consumerResult);
                await producerService.ProduceAsync(stoppingToken, message);
            }

            logger.LogInformation(
                $"Получили сообщение '{consumerResult.Message.Value}' " +
                $"со смещением '{consumerResult.Offset}'");
        }
        catch (Confluent.Kafka.ConsumeException ex)
        {
            // ошибка Kafka (нет топика, брокер недоступен и т.п.) — не валим сервис
            logger.LogError(ex, "Kafka consume error");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // нормальная остановка сервиса
            logger.LogInformation("ConsumerService остановлен.");
            break;
        }
        catch (Exception ex)
        {
            // любая другая ошибка — логируем и пробуем дальше
            logger.LogError(ex, "Unexpected error in ConsumerService");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
}