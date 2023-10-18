using Chr.Avro.Confluent;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kenso.Data.Kafka
{
    public class Consumer<TK, TV> : IDisposable
    {
        private readonly ILogger<Consumer<TK, TV>> _logger;
        private readonly KafkaOptions _kafkaOptions;
        private readonly IMessageHandler<TK, TV> _messageHandler;
        private IConsumer<TK, TV>? _consumer;

        public Consumer(ILogger<Consumer<TK, TV>> logger, IOptions<KafkaOptions> kafkaOptions, IMessageHandler<TK, TV> messageHandler)
        {
            _logger = logger;
            _kafkaOptions = kafkaOptions.Value;
            _messageHandler = messageHandler;

            if (string.IsNullOrEmpty(kafkaOptions.Value.SchemaRegistries))
            {
                throw new ArgumentException("SchemaRegistries not provided.");
            }

            if (string.IsNullOrEmpty(kafkaOptions.Value.Topic))
            {
                throw new ArgumentException("Topic not provided.");
            }

            if (string.IsNullOrEmpty(kafkaOptions.Value.BootstrapServers))
            {
                throw new ArgumentException("Kafka:ProducerSettings:BootstrapServers setting nor provided!");
            }
        }

        public async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting consumer loop.");
            _consumer ??= Init();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(cancellationToken);
                    await _messageHandler.Handle(cr.Message);
                    _consumer.Commit(cr);
                    _logger.LogInformation($"{cr.Message.Key}: {cr.Message.Value}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    _logger.LogError($"Consume error: {e.Error.Reason}. Message {e.Message}" );

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Unexpected error: {e}");
                    break;
                }
            }
        }

        public IConsumer<TK, TV> Init()
        {
            _logger.LogInformation($"Initializing consumer for topic:{_kafkaOptions.Topic}, groupId: {_kafkaOptions.GroupId}, bootstrapServers: {_kafkaOptions.BootstrapServers}");

            var registryClient = new CachedSchemaRegistryClient(
                new SchemaRegistryConfig
                {
                    Url = _kafkaOptions.SchemaRegistries,
                    EnableSslCertificateVerification = _kafkaOptions.EnableSslCertificateVerification,
                    SslCaLocation = _kafkaOptions.SslCaLocation,
                    BasicAuthUserInfo = _kafkaOptions.BasicAuthUserInfo
                });
            var consumer = CreateConsumer(registryClient);
            consumer.Subscribe(_kafkaOptions.Topic);
            return consumer;
        }

        private IConsumer<TK, TV> CreateConsumer(ISchemaRegistryClient registryClient)
        {
            return new ConsumerBuilder<TK, TV>(
                    new ConsumerConfig
                    {
                        BootstrapServers = _kafkaOptions.BootstrapServers,
                        SecurityProtocol = _kafkaOptions.SecurityProtocol,
                        SslCaLocation = _kafkaOptions.SslCaLocation,
                        SslCertificateLocation = _kafkaOptions.SslCertificateLocation,
                        SslKeyLocation = _kafkaOptions.SslKeyLocation,
                        SslEndpointIdentificationAlgorithm = _kafkaOptions.SslEndpointIdentificationAlgorithm,
                        EnableAutoCommit = false,
                        GroupId = _kafkaOptions.GroupId
                    })
                .SetAvroKeyDeserializer(registryClient)
                .SetAvroValueDeserializer(registryClient)
                
                .Build();
        }

        public void Dispose()
        {
            if (_consumer == null) return;
            _consumer.Close(); // Commit offsets and leave the group cleanly.
            _consumer.Dispose();
        }
    }
}
