using Confluent.Kafka;

namespace Kenso.Data.Kafka;

public class KafkaOptions
{
    public required string Topic { get; set; }
    public required string SchemaRegistries { get; set; }
    public int NumPartitions { get; set; } = 2;
    public short ReplicationFactor { get; set; } = 1;
    public string? BootstrapServers { get; set; }
    public string? GroupId { get; set; }
    public string? SslCaLocation { get; set; }
    public SecurityProtocol SecurityProtocol { get; set; }
    public string? SslCertificateLocation { get; set; }
    public string? SslKeyLocation { get; set; }
    public SslEndpointIdentificationAlgorithm SslEndpointIdentificationAlgorithm { get; set;}
    public bool EnsureTopicExists { get; set; } = false;
    public string? BasicAuthUserInfo { get; set; }
    public bool? EnableSslCertificateVerification { get; set; }
}