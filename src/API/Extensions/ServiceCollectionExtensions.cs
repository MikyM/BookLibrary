using System.Threading.RateLimiting;
using Asp.Versioning;
using DataAccess;
using EFCoreSecondLevelCacheInterceptor;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

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
    
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEFSecondLevelCache(opt =>
        {
            #if DEBUG
            opt.ConfigureLogging(true);
            #endif
            
            opt.UseEasyCachingCoreProvider("Default");
        });

        services.AddEasyCaching(opt =>
        {
            opt.UseInMemory("Default");
            
            /* configure distributed cache
            
            opt.UseRedisLock();
            
            opt.UseRedis(ropt =>
            {
                ropt.EnableLogging = true;
                
                // configure rest...
            }, "Redis");*/
            
            // enable hybrid cache
        });
        
        services.AddDbContext<IBookLibraryDbContext, BookLibraryDbContext>((provider,opt) =>
        {
            opt.UseSnakeCaseNamingConvention();
            opt.AddInterceptors(provider.GetRequiredService<SecondLevelCacheInterceptor>());

            opt.UseNpgsql(configuration.GetConnectionString("dataContext"), pg =>
            {
                // simple retry strategy
                pg.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
            });
        });
        
        return services;
    }
}