using Kenso.Data.Kafka;
using Kenso.Domain;
using SampleConsumer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<KafkaOptions>().BindConfiguration("Kafka");
        services.AddSingleton<Consumer<string, MeasurementRecord>>();
        services.AddSingleton<IMessageHandler<string, MeasurementRecord>, KafkaMessageHandler>();
        services.AddHostedService<Worker>();

    })
    .Build();

host.Run();