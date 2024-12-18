using System.Net;
using API;
using API.Extensions;
using Application.Json;
using Application.MediatR.Behaviors;
using Application.MediatR.Queries.Book;
using Application.Services;
using Application.Validators;
using Asp.Versioning.ApiExplorer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Keycloak.AuthServices.Common;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using StackExchange.Redis;

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
    
    builder.Services.AddEndpointsApiExplorer();

    var jsonConfigure = new BookLibraryJsonOptionsConfigurator();

    builder.Services.ConfigureHttpJsonOptions(x => jsonConfigure.Configure(x.SerializerOptions));
    
    var keycloakOptions = builder.Configuration.GetSection("Keycloak").Get<KeycloakInstallationOptions>() ?? throw new InvalidOperationException("Missing Keycloak installation");
    
    builder.Services.ConfigureAuth(builder.Configuration, keycloakOptions); 
    
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
        {
            OpenIdConnectUrl = new Uri($"{keycloakOptions.AuthServerUrl}/realms/" + keycloakOptions.Realm + "/.well-known/openid-configuration"),
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri($"{keycloakOptions.AuthServerUrl?.TrimEnd('/')}/realms/{keycloakOptions.Realm}/protocol/openid-connect/token"),
                    AuthorizationUrl = new Uri($"{keycloakOptions.AuthServerUrl?.TrimEnd('/')}/realms/{keycloakOptions.Realm}/protocol/openid-connect/auth"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "openid" },
                        { "profile", "profile" },
                        { "book-library-api", "book-library-api" }
                    }
                }
            }
        });
    
        OpenApiSecurityScheme keycloakSecurityScheme = new()
        {
            Reference = new OpenApiReference
            {
                Id = "Keycloak",
                Type = ReferenceType.SecurityScheme,
            },
            In = ParameterLocation.Header,
            Name = "Bearer",
            Scheme = "Bearer",
        };

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { keycloakSecurityScheme, Array.Empty<string>() },
        });
    });

    builder.Services.ConfigureVersioning();
    
    var redisConfig = new ConfigurationOptions()
    {
        EndPoints = [ new DnsEndPoint(builder.Configuration.GetValue<string>("Redis:Host")!, builder.Configuration.GetValue<int>("Redis:Port") ) ],
        Password = builder.Configuration.GetValue<string>("Redis:Password")
    };

    var multiplexer = await ConnectionMultiplexer.ConnectAsync(redisConfig);

    builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

    builder.Services.ConfigureDatabase(builder.Configuration, redisConfig);

    builder.Services.ConfigureRateLimiting();

    builder.Services.AddProblemDetails();

    builder.Services.AddExceptionHandler(opt =>
    {
        opt.AllowStatusCode404Response = true;
    });

    builder.Services.AddHostedService<InitializationService>();

    builder.Services.AddValidatorsFromAssembly(typeof(AddBookPayloadValidator).Assembly);

    builder.Services.ConfigureHealthChecks(builder.Configuration, keycloakOptions);

    builder.Services.AddMediatR(opt =>
    {
        opt.AddOpenBehavior(typeof(LoggingBehavior<,>));

        opt.RegisterGenericHandlers = true;

        opt.RegisterServicesFromAssembly(typeof(GetBookById).Assembly);
    });

    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(x =>
    {
        x.RegisterAssemblyModules(typeof(AutofacModule).Assembly);
    }));

    var app = builder.Build();
    
    app.UseSerilogRequestLogging(opt =>
    {
        opt.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    var versionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(opt =>
        {
            foreach (var desc in versionProvider.ApiVersionDescriptions)
            {
                opt.SwaggerEndpoint($"{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
            }
            
            opt.OAuthClientSecret(keycloakOptions.Credentials.Secret);
            opt.OAuthClientId(keycloakOptions.Resource);
            opt.OAuthAppName("Book Library API");
            opt.OAuthScopeSeparator(" ");
            opt.OAuthScopes("book-library-api");
            opt.OAuthUsePkce();
        });
    }
    
    app.UseExceptionHandler();

    app.UseStatusCodePages();
    
    //app.UseHttpsRedirection();

    app.UseRateLimiter();

    app.UseCors(AuthPolicies.CorsPolicy);
    
    app.UseAuthentication();
    
    app.UseAuthorization();
    
    app.UseResponseCaching();

    app.MapBookLibrary();

    app.Run();
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
