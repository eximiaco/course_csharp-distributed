using Asp.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Serilog.Sinks.OpenTelemetry;
using Serilog.Enrichers.OpenTelemetry;
using M3.Desafio.SeedWork.Telemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using M3.Desafio.Inscricoes.Eventos;
using M3.Desafio.SeedWork.ServiceBus.Silverback;
using Silverback.Messaging.Configuration;
using M3.Desafio.Inscricoes.Telemetria;
using Microsoft.Extensions.Options;
using Google.Protobuf.WellKnownTypes;
using Asp.Versioning.ApiExplorer;

namespace M3.Desafio.Inscricoes.API.Infrastructure;

internal static class ServicesExtensions
{
    public static IServiceCollection AddSwaggerDoc(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(x => x.FullName); // Evita conflitos de nomes de schema
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                }
            );

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                }
            );

            // Gera um documento Swagger para cada versão da API
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerDoc(description.GroupName, new OpenApiInfo()
                {
                    Title = $"Minha API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString()
                });
            }
        });
        return services;
    }

    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(config =>
        {
            config.ReportApiVersions = true; // Inclui as versões suportadas no cabeçalho da resposta
            config.AssumeDefaultVersionWhenUnspecified = true; // Define uma versão padrão se nenhuma for especificada
            config.DefaultApiVersion = new ApiVersion(1); // Versão padrão
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V"; // Formato: "v1", "v2", etc.
            options.SubstituteApiVersionInUrl = true;
        });
        return services;
    }

    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(
            o =>
                o.AddPolicy(
                    "default",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                )
        );
        return services;
    }

    public static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();
        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "ready" });
        return services;
    }

    public static IServiceCollection AddLogs(this IServiceCollection services, IConfiguration configuration, string serviceName)
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
                options.Endpoint = "http://localhost:4317";
                options.IncludedData = IncludedData.MessageTemplateTextAttribute
                    | IncludedData.TraceIdField | IncludedData.SpanIdField;
                options.Protocol = OtlpProtocol.Grpc;
            })
            .CreateLogger();

        services.AddSingleton(Log.Logger);
        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
        services
            .AddControllers(o =>
            {
                o.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });
        return services;
    }

    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection kafkaSection = configuration.GetSection("Kafka");
        var kafkaConfig = new KafkaConfig();
        kafkaConfig.Connection = kafkaSection.GetSection("Connection").Get<KafkaConnectionConfig>()!;
        services.AddSingleton(kafkaConfig);

        services
            .AddSilverback()
            .WithConnectionToMessageBroker(config => config.AddKafka())
            .AddKafkaEndpoints(endpoints => endpoints
                .Configure(config => config.Configure(kafkaConfig))
                .AddOutbound<InscricaoRealizadaEvento>(endpoint => endpoint
                    .ProduceTo("inscricoes")
                    .WithKafkaKey<InscricaoRealizadaEvento>(envelope => envelope.Message!.Id)
                    .SerializeAsJson(serializer => serializer.UseFixedType<InscricaoRealizadaEvento>())
                    .DisableMessageValidation()));
        return services;
    }

    public static IServiceCollection AddTelemetry(this IServiceCollection serviceCollection, string serviceName, string serviceVersion, IConfiguration configuration)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        TelemetrySettings settings;
        if (configuration.GetSection("OpenTelemetry") is var section && section.Exists())
            settings = new TelemetrySettings(serviceName, serviceVersion, new TelemetryExporter(section["Type"] ?? string.Empty, section["Endpoint"] ?? string.Empty));
        else
            settings = new TelemetrySettings(serviceName, serviceVersion, new TelemetryExporter("console", ""));

        InscricoesOtelMetrics metrics = new();

        serviceCollection.AddSingleton(settings);
        serviceCollection.AddScoped(sp => new OtelTracingService(sp.GetService<TelemetrySettings>()));
        serviceCollection.AddScoped<TelemetryFactory>();
        serviceCollection.AddSingleton(metrics);
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
                        //config.Endpoint = new Uri("http://ec2-52-54-217-123.compute-1.amazonaws.com:4317");
                        config.Protocol = OtlpExportProtocol.Grpc;
                    });
            })
            .WithMetrics(builder =>
            {
                builder
                    .ConfigureResource(configureResource)
                    .AddMeter(metrics.Name)
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(config =>
                    {
                        config.Endpoint = new Uri(settings.Exporter.Endpoint);
                        config.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
        return serviceCollection;
    }
}