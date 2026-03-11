using Confluent.Kafka.Extensions.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.OpenTelemetry;

public static class DiExtensions
{
    public static ILoggingBuilder AddOpenTelemetryLogger(this ILoggingBuilder builder) =>
        builder.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;
        });

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, string serviceName) =>
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithMetrics(metrics =>
                metrics
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsqlInstrumentation())
            .WithTracing(tracing =>
                tracing
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddConfluentKafkaInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddNpgsql())
            .UseOtlpExporter().Services;
}