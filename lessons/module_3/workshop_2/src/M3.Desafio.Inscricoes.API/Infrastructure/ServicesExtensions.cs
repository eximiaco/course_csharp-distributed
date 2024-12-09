using Asp.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog.Filters;
using Serilog;
using System.Reflection;
using Serilog.Sinks.OpenTelemetry;
using static CSharpFunctionalExtensions.Result;
using Serilog.Enrichers.OpenTelemetry;
using M3.Desafio.SeedWork.Telemetry;
using M3.Desafio.Inscricoes.Telemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;

namespace M3.Desafio.Inscricoes.API.Infrastructure;

internal static class ServicesExtensions
{
    public static IServiceCollection AddSwaggerDoc(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(x => x.ToString());
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

            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Cdc Consumer",
                    Description = "Consumer consolidator worker.",
                    Version = "v1"
                }
            );
        });
        return services;
    }

    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
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
        // hcBuilder
        //     .AddNpgSql(
        //         configuration.GetConnectionString(Ambient.DatabaseConnectionName)!,
        //         name: "integration-store-check",
        //         tags: new string[] {"IntegrationStoreCheck", "health"});
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
        var assembly = Assembly.Load(typeof(Inscricao).Assembly.ToString());
        services
            .AddControllers(o =>
            {
                o.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddApplicationPart(assembly);
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
        serviceCollection.AddSingleton(metrics);
        serviceCollection.AddSingleton<InscricoesOtelVariables>();

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
                        //config.Endpoint = new Uri("http://3.82.16.160:4317");
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
                        config.Endpoint = new Uri(settings.Exporter.Endpoint ?? string.Empty);
                        //config.Endpoint = new Uri("http://3.82.16.160:4317");
                        config.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
        return serviceCollection;
    }
}