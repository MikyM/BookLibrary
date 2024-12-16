using API;
using API.Extensions;
using Application.MediatR.Behaviors;
using Application.MediatR.Queries.Book;
using Application.Services;
using Application.Validators;
using FluentValidation;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateBootstrapLogger();

Log.Logger.Information("Application starting up..");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();

    builder.Services.AddSerilog(x => x.ReadFrom.Configuration(builder.Configuration));
    
    builder.Services.AddLogging(x => x.AddSerilog());

    builder.Services.AddCors(x =>
        x.AddPolicy(AuthPolicies.CorsPolicy, y =>
        {
            y.AllowCredentials();
            y.AllowAnyHeader();
            y.SetIsOriginAllowed(_ => true);
        }));

    builder.Services.ConfigureAuth(builder.Configuration);
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.ConfigureVersioning();

    builder.Services.ConfigureDatabase(builder.Configuration);

    builder.Services.ConfigureRateLimiting();

    builder.Services.AddExceptionHandler(opt =>
    {
        opt.AllowStatusCode404Response = true;
    });

    builder.Services.AddHostedService<InitializationService>();

    builder.Services.AddValidatorsFromAssembly(typeof(GridifyQueryValidator).Assembly);

    builder.Services.AddMediatR(opt =>
    {
        opt.AddBehavior(typeof(LoggingBehavior<,>));
        opt.AddBehavior(typeof(ExceptionToResultBehavior<,>));

        opt.RegisterServicesFromAssembly(typeof(GetBookById).Assembly);
    });

    var app = builder.Build();
    
    app.UseSerilogRequestLogging(opt =>
    {
        opt.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseRateLimiter();

    app.UseCors(AuthPolicies.CorsPolicy);

    app.UseAuthorization();
    app.UseAuthentication();

    app.UseStatusCodePages();
    
    app.UseExceptionHandler();
    
    app.UseResponseCaching();

    app.MapBookLibrary();

    app.MapHealthChecks("health");

    app.Run();
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Application terminated unexpectedly");
    throw;
}