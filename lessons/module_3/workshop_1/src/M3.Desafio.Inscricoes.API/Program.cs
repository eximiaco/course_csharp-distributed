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
        .AddSwaggerDoc()
        .AddVersioning()
        .AddCustomCors()
        .AddOptions()
        .AddTransient<UnitOfWorkMiddleware>()
        .AddMessageBroker(builder.Configuration)
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
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
        });
    }

    app.UseHttpsRedirection();
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