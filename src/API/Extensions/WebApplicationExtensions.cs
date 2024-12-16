using Asp.Versioning;
using Asp.Versioning.Builder;
using Gridify;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

public static class WebApplicationExtensions
{
    private static void MapInventoryRelated(this RouteGroupBuilder app)
    {
        var bookGroup = app.MapGroup("book")
            .WithTags("Books");

        bookGroup.MapGet("", (HttpContext context) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetBooksPolicy)
            .WithName("GetBooks")
            .WithOpenApi()
            .WithSummary("Returns a collection of books");
        
        bookGroup.MapGet("{id:long}", (HttpContext context, [FromRoute] long id) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetBooksPolicy)
            .WithName("GetBook")
            .WithOpenApi()
            .WithSummary("Returns a book by given ID");
        
        bookGroup.MapGet("search", (HttpContext context, [AsParameters] GridifyQuery query) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetBooksPolicy)
            .WithName("SearchBooks")
            .WithOpenApi()
            .WithSummary("Searches for books based on given parameters");
        
        bookGroup.MapPost("", (HttpContext context, [FromBody] GridifyQuery query) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.PostBooksPolicy)
            .WithName("AddBook")
            .WithOpenApi()
            .WithSummary("Adds a book");
    }

    private static void MapOrderRelated(this RouteGroupBuilder app)
    {
        var orderGroup = app.MapGroup("order")
            .WithTags("Orders");

        orderGroup.MapGet("", (HttpContext context) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetOrdersPolicy)
            .WithName("GetOrders")
            .WithOpenApi()
            .WithSummary("Returns a collection of orders");
                
        orderGroup.MapGet("{id:guid}", (HttpContext context, [FromRoute] Guid id) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetOrdersPolicy)
            .WithName("GetOrder")
            .WithOpenApi()
            .WithSummary("Returns an order by given ID");
    }
    
    private static void MapAuthorRelated(this RouteGroupBuilder app)
    {
        var authorGroup = app.MapGroup("author")
            .WithTags("Authors");

        authorGroup.MapGet("", (HttpContext context) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetAuthorsPolicy)
            .WithName("GetAuthors")
            .WithOpenApi()
            .WithSummary("Returns a collection of authors");
        
        authorGroup.MapGet("{id:long}", (HttpContext context, [FromRoute] long id) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetAuthorsPolicy)
            .WithName("GetAuthor")
            .WithOpenApi()
            .WithSummary("Returns an author by given ID");
        
        authorGroup.MapGet("search", (HttpContext context, [AsParameters] GridifyQuery query) => Results.Ok())
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetAuthorsPolicy)
            .WithName("SearchAuthors")
            .WithOpenApi()
            .WithSummary("Searches for authors based on given parameters");
    }
    
    public static WebApplication MapBookLibrary(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var baseGroup = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet)
            .RequireCors(AuthPolicies.CorsPolicy)
            .RequireAuthorization(AuthPolicies.DefaultAccessPolicy);
        
        baseGroup.MapOrderRelated();

        baseGroup.MapInventoryRelated();
        
        baseGroup.MapAuthorRelated();

        return app;
    }
}