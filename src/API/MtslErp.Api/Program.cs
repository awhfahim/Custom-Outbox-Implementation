using DotNetEnv;
using MassTransit;
using MtslErp.Api.Extensions;
using MtslErp.Common.Application;
using MtslErp.Common.Application.Extensions;
using MtslErp.Common.Infrastructure;
using PrintFactoryManagement.HttpApi;
using Serilog;
using Serilog.Events;

Env.Load();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddModuleConfiguration(["printFactory"]); // Add module configuration name here
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddSerilog((ctx, lc) => lc
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .WriteTo.ConfigureEmailSink(builder.Configuration)
        .WriteTo.Console()
    );

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(); // To be Implemented
    builder.Services.RegisterPrintFactoryManagementHttpApi(builder.Configuration);

    builder.Services.AddCommonInfrastructureServices(builder.Configuration);
    builder.Services.AddCommonApplicationServices();
    builder.Services.RegisterMasstransit(builder.Configuration);


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return 1;
}
