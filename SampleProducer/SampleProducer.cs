using Confluent.Kafka;
using Kenso.Data.Kafka;
using Kenso.Domain;
using Microsoft.Extensions.Logging;

namespace SampleProducer;

public class SampleProducer
{

    private readonly DependentProducer<string, MeasurementRecord> _kafkaProducer;
    private readonly ILogger<SampleProducer> _logger;

    public SampleProducer(ILogger<SampleProducer> logger, DependentProducer<string, MeasurementRecord> kafkaProducer)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }

    public void PublishMeasurement(MeasurementRecord measurement)
    {
        var key = $"{measurement.AssetName}:{measurement.PartNumber}:{measurement.CharacteristicName}";
        _kafkaProducer.Produce(new Message<string, MeasurementRecord>
            {
                Key = key,
                Value = measurement
            },
            (deliveryReport) =>
            {
                if (deliveryReport.Error.Code != ErrorCode.NoError)
                {
                    _logger.LogError($"Failed to deliver message: {deliveryReport.Error.Reason}");

                }
                else
                {
                    _logger.LogInformation($"Produced event: key = {key}");
                }
            });
    }
}