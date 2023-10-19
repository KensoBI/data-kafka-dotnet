using Kenso.Data.Kafka;
using Kenso.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleProducer;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<KafkaOptions>().BindConfiguration("Kafka");
        services.AddSingleton<ClientHandle>();
        services.AddSingleton<DependentProducer<string, MeasurementRecord>>();
        services.AddSingleton<Consumer<string, MeasurementRecord>>();
        services.AddSingleton<IMessageHandler<string, MeasurementRecord>, SampleMessageHandler>();
        services.AddSingleton<SampleProducer.SampleProducer>();
    })
    .Build();

var producer = host.Services.GetRequiredService<DependentProducer<string, MeasurementRecord>>();
await producer.BuildProducer();

var sampleProducer = host.Services.GetRequiredService<SampleProducer.SampleProducer>();
sampleProducer.PublishMeasurement(new MeasurementRecord
{
    MeasurementValue = 10.1m,
    MeasurementDate = DateTime.Now,
    CharacteristicName = "testCharacteristic",
    PartName = "test-part",
    ModelName = "exampleModel",
    PartNumber = "12345",
    FeatureName = "exampleFeature",
    Nominal = 10.5m,
    Usl = 15.0m,
    Lsl = 5.0m,
    UslWarn = 14.0m,
    LslWarn = 6.0m,
    Unit = "mm",
    Deviation = 1.2m,
    Device = "exampleDevice",
    Operator = "exampleOperator"
});

Console.ReadLine();
