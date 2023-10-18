using Confluent.Kafka;
using Kenso.Data.Kafka;
using Kenso.Domain;

namespace SampleConsumer;

public class KafkaMessageHandler : IMessageHandler<string, MeasurementRecord>
{
    private readonly ILogger<KafkaMessageHandler> _logger;

    public KafkaMessageHandler(ILogger<KafkaMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(Message<string, MeasurementRecord> message)
    {
        _logger.LogInformation("Handled message: key:" + message.Key);
        return Task.CompletedTask;
    }
}