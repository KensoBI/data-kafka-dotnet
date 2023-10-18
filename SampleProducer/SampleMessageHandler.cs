using Confluent.Kafka;
using Kenso.Data.Kafka;
using Kenso.Domain;

namespace SampleProducer
{
    public class SampleMessageHandler : IMessageHandler<string, MeasurementRecord>
    {
        public Task Handle(Message<string, MeasurementRecord> cr)
        {
            Console.WriteLine("message received: " + cr.Value);
            return null;
        }
    }
}
