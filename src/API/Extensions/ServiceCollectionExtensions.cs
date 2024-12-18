using Asp.Versioning;
using DataAccess;
using EasyCaching.Redis;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using EFCoreSecondLevelCacheInterceptor;
using Gridify;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using StackExchange.Redis;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration, KeycloakInstallationOptions keycloakOptions)
    {
        services.AddKeycloakWebApiAuthentication(configuration, opt =>
        {
            var useSsl = keycloakOptions.SslRequired != "none";
            
            opt.RequireHttpsMetadata = useSsl;

            if (useSsl)
            {
                return;
            }

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            
            opt.BackchannelHttpHandler = handler;
        });

        const string realmRole = "book-library-accessor";
        const string getterRole = "getter";
        const string posterRole = "poster";
        
        services.AddAuthorization(opt =>
        {
            /*opt.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRealmRoles(realmRole)
                .Build();*/
            
            opt.AddPolicy(AuthPolicies.DefaultAccessPolicy, policy =>
            {
                policy.RequireRealmRoles(realmRole);
            });
            
            opt.AddPolicy(AuthPolicies.GetAuthorsPolicy, policy =>
            {
                policy.RequireRealmRoles(realmRole);
                policy.RequireResourceRoles(getterRole);
            });
            
            opt.AddPolicy(AuthPolicies.GetBooksPolicy, policy =>
            {
                policy.RequireRealmRoles(realmRole);
                policy.RequireResourceRoles(getterRole);
            });
            
            opt.AddPolicy(AuthPolicies.PostBooksPolicy, policy =>
            {
                policy.RequireRealmRoles(realmRole);
                policy.RequireResourceRoles(posterRole);
            });
            
            opt.AddPolicy(AuthPolicies.GetOrdersPolicy, policy =>
            {
                policy.RequireRealmRoles(realmRole);
                policy.RequireResourceRoles(getterRole);
            });
            
        }).AddKeycloakAuthorization(configuration);

        return services;
    }
    
    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration, KeycloakInstallationOptions keycloakOptions)
    {
        services.AddHealthChecks()
            .AddNpgSql(x => configuration.GetConnectionString("DataContext") ?? throw new NullReferenceException())
            .AddRedis(x => x.GetRequiredService<IConnectionMultiplexer>())
            .AddIdentityServer(new Uri(keycloakOptions.AuthServerUrl ?? throw new NullReferenceException()),
                "/realms/" + keycloakOptions.Realm + "/.well-known/openid-configuration", "Keycloak");

        return services;
    }

    public static IServiceCollection ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.ApiVersionReader = new UrlSegmentApiVersionReader();
            opt.UnsupportedApiVersionStatusCode = 501;
            opt.ReportApiVersions = true;
            opt.DefaultApiVersion = ApiVersion.Default;
            opt.AssumeDefaultVersionWhenUnspecified = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
    
    public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services)
    {
        // configure simple bucket based ratelimiter
        services.AddRateLimiter(opt =>
        {
            opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            opt.AddTokenBucketLimiter("token", options =>
            {
                options.TokenLimit = 1000;
                options.ReplenishmentPeriod = TimeSpan.FromHours(1);
                options.TokensPerPeriod = 700;
                options.AutoReplenishment = true;
            });
        });
        
        return services;
    }
    
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration, ConfigurationOptions redisOptions)
    {
        services.AddEFSecondLevelCache(opt =>
        {
            #if DEBUG
            opt.ConfigureLogging(true);
            #endif

            opt.UseEasyCachingCoreProvider("Hybrid", true);

            opt.CacheAllQueries(CacheExpirationMode.Sliding, TimeSpan.FromMinutes(5));
        });

        services.AddEasyCaching(opt =>
        {
            opt.WithSystemTextJson("JsonSerializer");
            
            opt.UseInMemory(mopt =>
            {
                mopt.SerializerName = "JsonSerializer";
            }, "Default");
            
            opt.UseRedisLock();
            
            opt.UseRedis(ropt =>
            {
                ropt.SerializerName = "JsonSerializer";
                
                ropt.EnableLogging = true;

                ropt.DBConfig = new RedisDBOptions
                {
                    ConfigurationOptions = redisOptions
                };
            }, "Redis");
            
            // combine local and distributed
            opt.UseHybrid(config =>
                {
                    config.TopicName = "cache-topic";
                    config.EnableLogging = false;


                    config.LocalCacheProviderName = "Default";

                    config.DistributedCacheProviderName = "Redis";
                }, "Hybrid")
                // use redis bus
                .WithRedisBus(busConf => 
                {
                    busConf.ConfigurationOptions = redisOptions;
                    busConf.SerializerName = "JsonSerializer";
                });
        });
        
        services.AddDbContext<IBookLibraryDbContext, BookLibraryDbContext>((provider,opt) =>
        {
            opt.UseSnakeCaseNamingConvention();
            opt.AddInterceptors(provider.GetRequiredService<SecondLevelCacheInterceptor>());

            opt.UseNpgsql(configuration.GetConnectionString("DataContext"), pg =>
            {
                // simple retry strategy
                pg.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
            })
            .ReplaceService<IHistoryRepository, MigrationHistory>();
        });
        
        GridifyGlobalConfiguration.EnableEntityFrameworkCompatibilityLayer();
        
        return services;
    }
}