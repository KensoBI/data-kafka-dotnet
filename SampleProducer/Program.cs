
using System.Runtime.InteropServices.JavaScript;
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

/*
 * 
 * public class Program
   {
   public static void Main(string[] args)
   {
   //sampleProducer.PublishMeasurement(new MeasurementRecord
   //{
   //    MeasurementValue = 10.1m,
   //    MeasurementDate = DateTime.Now,
   //    CharacteristicName = "testCharacteristic",
   //    PartName = "test-part",
   //    ModelName = "exampleModel",
   //    PartNumber = "12345",
   //    FeatureName = "exampleFeature",
   //    Nominal = 10.5m,
   //    Usl = 15.0m,
   //    Lsl = 5.0m,
   //    UslWarn = 14.0m,
   //    LslWarn = 6.0m,
   //    Unit = "mm",
   //    Deviation = 1.2m,
   //    Device = "exampleDevice",
   //    Operator = "exampleOperator"
   //});
   
   ProducerBasic();
   Console.ReadLine();
   }
   
   
   public static void ProducerBasic()
   {
   const string topic = "client1-topic";
   var producerConfig = new ProducerConfig
   {
   BootstrapServers = "stream.kensobi.com:9095",
   //SaslMechanism = SaslMechanism.Plain,
   SecurityProtocol = SecurityProtocol.Ssl,
   //// Note: If your root CA certificates are in an unusual location you
   //// may need to specify this using the SslCaLocation property.
   //SaslUsername = "<ccloud key>",
   //SaslPassword = "<ccloud secret>"
   SslCaLocation = "D:\\DEV\\data-kafka-dotnet\\SampleClient\\certs\\client\\ca.crt",
   SslCertificateLocation ="D:\\DEV\\data-kafka-dotnet\\SampleClient\\certs\\client\\client1_client.pem",
   SslKeyLocation = "D:\\DEV\\data-kafka-dotnet\\SampleClient\\certs\\client\\\\client1_client.key",
   SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.None
   };
   
   using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
   //using (var producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build())
   {
   var numProduced = 0;
   Random rnd = new Random();
   const int numMessages = 1;
   
   //var user = users[rnd.Next(users.Length)];
   // var item = items[rnd.Next(items.Length)];
   
   producer.Produce(topic, new Message<string, string> { Key = "testKey1", Value = "yooo2" },
   (deliveryReport) =>
   {
   if (deliveryReport.Error.Code != ErrorCode.NoError)
   {
   Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
   }
   else
   {
   // Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
   numProduced += 1;
   }
   });
   
   
   producer.Flush(TimeSpan.FromSeconds(10));
   Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
   }
   }
   
   public void Consumer()
   {
   //using var registryClient = new CachedSchemaRegistryClient(
   //    new SchemaRegistryConfig
   //    {
   //        Url = SchemaRegistries
   //    });
   //using var consumer = CreateConsumer(registryClient);
   //consumer.Subscribe(Topic);
   //// await ProducerNoKey();
   //// await ProducerGuid();
   
   //await ProducerDependent();
   
   
   //var result = consumer.Consume();
   //Console.WriteLine("result");
   }
   }
*/