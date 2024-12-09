using Autofac.Extensions.DependencyInjection;
using Autofac;
using M3.Desafio.Inscricoes.API.Infrastructure;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var assemblyName = Assembly.GetExecutingAssembly().GetName();
var serviceName = assemblyName.Name;
var serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

try
{
    Log.ForContext("ApplicationName", serviceName).Information("Starting application");
    builder.Services
        .AddTelemetry(serviceName!, serviceVersion!, builder.Configuration)
        .AddLogs(builder.Configuration, serviceName!)
        .AddHttpContextAccessor()
        .AddEndpointsApiExplorer()
        .AddSwaggerDoc()
        .AddVersioning()
        .AddCustomCors()
        .AddOptions()
        .AddTransient<UnitOfWorkMiddleware>()
        .AddCustomMvc();

    builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new ApplicationModule());
    });
    builder.Host.UseSerilog();
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<UnitOfWorkMiddleware>();
    app.MapControllers();
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.ForContext("ApplicationName", serviceName)
        .Fatal(ex, "Program terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}