using Asp.Versioning;
using DataAccess;
using EasyCaching.Core.Configurations;
using EasyCaching.Redis;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using EFCoreSecondLevelCacheInterceptor;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKeycloakWebApiAuthentication(configuration);
        
        services.AddAuthorization(opt =>
        {
            opt.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRealmRoles("book_library_accessor")
                .Build();
            
            opt.AddPolicy(AuthPolicies.DefaultAccessPolicy, policy =>
            {
                policy.RequireRealmRoles("book_library_accessor");
                policy.RequireAuthenticatedUser();
            });
            
            opt.AddPolicy(AuthPolicies.GetAuthorsPolicy, policy =>
            {
                policy.RequireResourceRoles("author_getter");
                policy.RequireAuthenticatedUser();
            });
            
            opt.AddPolicy(AuthPolicies.GetBooksPolicy, policy =>
            {
                policy.RequireResourceRoles("book_getter");
                policy.RequireAuthenticatedUser();
            });
            
            opt.AddPolicy(AuthPolicies.PostBooksPolicy, policy =>
            {
                policy.RequireResourceRoles("books_poster");
                policy.RequireAuthenticatedUser();
            });
            
            opt.AddPolicy(AuthPolicies.GetOrdersPolicy, policy =>
            {
                policy.RequireResourceRoles("orders_getter");
                policy.RequireAuthenticatedUser();
            });
            
        }).AddKeycloakAuthorization(configuration);

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
            options.GroupNameFormat = "'v'V";
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
            });
        });
        
        return services;
    }
}