using System.Text.Json;
using System.Text.Json.Serialization;
using MtslErp.Api.Extensions;
using MtslErp.Common.Application;
using MtslErp.Common.Application.Extensions;
using MtslErp.Common.Infrastructure;
using PrintFactoryManagement.HttpApi;
using Serilog.Events;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using MtslErp.Api;
using MtslErp.Common.Application.Options;
using MtslErp.Common.HttpApi.Others;
using SecurityManagement.Application;
using SecurityManagement.HttpApi;
using Serilog;

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

    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables();

    builder.Services.AddMemoryCache();

    builder.Configuration.AddModuleConfiguration([
        AvailableModule.PrintFactory, AvailableModule.SecurityManagement
    ]);

    builder.Services.AddSerilog((_, lc) => lc
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .WriteTo.ConfigureEmailSink(builder.Configuration)
        .WriteTo.Console()
    );

    if (builder.Environment.IsDevelopment())
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);
    }

    builder.Services.AddControllers(opts =>
            {
                opts.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                opts.OutputFormatters.RemoveType<StringOutputFormatter>();
                opts.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
            }
        )
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opts.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = ctx => ctx.MakeValidationErrorResponse();
        });


    var appOptions = configuration.GetRequiredSection(AppOptions.SectionName).Get<AppOptions>();

    ArgumentNullException.ThrowIfNull(appOptions);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(nameof(AppOptions.AllowedOriginsForCors), x => x
            .WithOrigins(appOptions.AllowedOriginsForCors)
            .WithExposedHeaders(SecurityManagementApplicationConstants.XsrfTokenHeaderKey)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    builder.Services.RegisterMasstransit(builder.Configuration);

    builder.Services.AddCommonInfrastructureServices(builder.Configuration);
    builder.Services.AddCommonApplicationServices();

    builder.Services.RegisterPrintFactoryManagement(builder.Configuration);
    await builder.Services.RegisterSecurityManagementAsync(builder.Configuration);


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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
