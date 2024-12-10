using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.OpenTelemetry;
using Serilog.Sinks.OpenTelemetry;
using M3.Desafio.SeedWork.ServiceBus.Silverback;
using M3.Desafio.SeedWork.Telemetry;
using OpenTelemetry.Exporter;
using M3.Desafio.Financeiro.Consumers;

namespace M3.Desafio.Financeiro.BrokerConsumer.Infrastructure;

internal static class ServicesExtensions
{
    internal static IServiceCollection AddLogs(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithOpenTelemetrySpanId()
            .Enrich.WithOpenTelemetryTraceId()
            .Enrich.WithProperty("service_name", serviceName)
            .WriteTo.OpenTelemetry(options =>
            {
                //options.Endpoint = "http://ec2-52-54-217-123.compute-1.amazonaws.com:4317";
                options.Endpoint = "http://localhost:4317";
                options.IncludedData = IncludedData.MessageTemplateTextAttribute
                    | IncludedData.TraceIdField | IncludedData.SpanIdField;
                options.Protocol = OtlpProtocol.Grpc;
            })
            .CreateLogger();

        services.AddSingleton(Log.Logger);
        return services;
    }

    internal static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }

    internal static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection kafkaSection = configuration.GetSection("Kafka");
        var kafkaConfig = new KafkaConfig();
        kafkaConfig.Connection = kafkaSection.GetSection("Connection").Get<KafkaConnectionConfig>()!;
        services.AddSingleton(kafkaConfig);
        services
            .AddSilverback()
            .WithConnectionToMessageBroker(config => config.AddKafka())
            .AddEndpointsConfigurator<KafkaEndpointsConfigurator>()
            .AddScopedSubscriber<GerarMensalidadesParaNovaInscricaoConsumer>();
        
        services.AddScoped<SilverbackServiceBus>();
        return services;
    }

    internal static IServiceCollection AddTelemetry(this IServiceCollection serviceCollection, string serviceName, string serviceVersion, IConfiguration configuration)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        TelemetrySettings settings;
        if (configuration.GetSection("OpenTelemetry") is var section && section.Exists())
            settings = new TelemetrySettings(serviceName, serviceVersion, new TelemetryExporter(section["Type"] ?? string.Empty, section["Endpoint"] ?? string.Empty));
        else
            settings = new TelemetrySettings(serviceName, serviceVersion, new TelemetryExporter("console", ""));

        serviceCollection.AddSingleton(settings);
        serviceCollection.AddScoped(sp => new OtelTracingService(sp.GetService<TelemetrySettings>()));
        serviceCollection.AddScoped<TelemetryFactory>();
        serviceCollection.AddSingleton<OtelVariables>();

        Action<ResourceBuilder> configureResource = r => r.AddService(
            serviceName: settings.ServiceName,
            serviceVersion: settings.ServiceVersion,
            serviceInstanceId: Environment.MachineName);

        serviceCollection
            .AddOpenTelemetry()
            .ConfigureResource(configureResource)
            .WithTracing(builder =>
            {
                builder
                    .AddSource(settings.ServiceName)
                    .AddSource("Silverback.Integration.Produce")
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(o =>
                    {
                        o.RecordException = true;
                        o.SetDbStatementForText = true;
                    })
                    .AddEntityFrameworkCoreInstrumentation(o => { })
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.EnrichWithHttpRequest = (a, r) => a?.AddTag("env", environmentName);
                        opts.RecordException = true;

                    })
                    .AddOtlpExporter(config =>
                    {
                        config.Endpoint = new Uri(settings.Exporter.Endpoint);
                        config.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
        return serviceCollection;
    }
}
