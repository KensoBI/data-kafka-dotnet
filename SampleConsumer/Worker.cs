using Kenso.Data.Kafka;
using Kenso.Domain;

namespace SampleConsumer
{
    public class Worker : BackgroundService
    {
        private readonly Consumer<string, MeasurementRecord> _consumer;

        public Worker(Consumer<string, MeasurementRecord> consumer)
        {
            _consumer = consumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _consumer.StartConsumerLoop(stoppingToken);
        }
    }
}