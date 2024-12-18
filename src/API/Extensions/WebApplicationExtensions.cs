using System.Text.Json;
using Application.MediatR.Commands.Book;
using Application.MediatR.Queries.Book;
using Application.MediatR.Queries.Order;
using Asp.Versioning;
using FluentValidation;
using Gridify;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Author;
using Models.Book;
using Models.Order;
using OneOf;
using Remora.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace API.Extensions;

public static class WebApplicationExtensions
{
    private static IResult TranslateRemoraResult<TEntity>(Remora.Results.Result<TEntity> result, int defaultSuccessCode = 200)
    {
        var code = result.DetermineHttpStatusCode(defaultSuccessCode);

        if (result.IsDefined(out var entity))
        {
            if (entity is IOneOf oneOf)
            {
                return Results.Json(oneOf.Value, (JsonSerializerOptions?)null, "application/json", code);
            }
            
            return Results.Json(entity, (JsonSerializerOptions?)null, "application/json", code);
        }

        return Results.StatusCode(code);
    }
    
    private static IResult TranslateRemoraResult(Remora.Results.Result result, int defaultSuccessCode = 200)
    {
        var code = result.DetermineHttpStatusCode(defaultSuccessCode);

        return Results.StatusCode(code);
    }

    private static int DetermineHttpStatusCode(this Remora.Results.IResult result, int defaultSuccessCode = 200)
    {
        if (result.IsSuccess)
        {
            return defaultSuccessCode;
        }

        return result.Error switch
        {
            NotFoundError _ => 404,
            _ => 500
        };
    }
    
    private static void MapInventoryRelated(this RouteGroupBuilder app)
    {
        var bookGroup = app.MapGroup("book")
            .WithTags("Books");

        bookGroup.MapGet("", async (CancellationToken token, [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(new GetAllBooks.Query(), token);

                return TranslateRemoraResult(result);
            })
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetBooksPolicy)
            .WithName("GetAllBooks")
            .WithOpenApi()
            .WithSummary("Returns a collection of books")
            .Produces<IEnumerable<IBookPayload>>()
            .Produces(StatusCodes.Status500InternalServerError);

        bookGroup.MapGet("{id:long}",
                async (CancellationToken token, [FromServices] IMediator mediator, [FromRoute] long id) =>
                {
                    var result = await mediator.Send(new GetBookById.Query(id), token);

                    return TranslateRemoraResult(result);
                })
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetBooksPolicy)
            .WithName("GetBook")
            .WithOpenApi()
            .WithSummary("Returns a book by given ID")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IBookPayload>()
            .Produces(StatusCodes.Status404NotFound);

        bookGroup.MapGet("search",
                async (CancellationToken token, [FromServices] IMediator mediator, [AsParameters] GridifyQuery query) =>
                {
                    var result = await mediator.Send(new GetBooksByGridifyQuery.Query(query), token);

                    return TranslateRemoraResult(result);
                })
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetBooksPolicy)
            .WithName("SearchBooks")
            .WithOpenApi()
            .WithSummary("Searches for books based on given parameters")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IExtendedPaging<IBookPayload>>();
        
        bookGroup.MapPost("", async ([FromBody] AddBookPayload payload, [FromHeader(Name = "Accept")] string? acceptHeader, IValidator<IAddBookPayload> validator, IMediator mediator) =>
            {
                var validationResult = await validator.ValidateAsync(payload);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                var shouldReturnCreated = acceptHeader == Constants.ReturnEntityHeaderValue;
                
                var result = await mediator.Send(new AddBook.Command(payload, shouldReturnCreated));
                
                return TranslateRemoraResult(result, StatusCodes.Status201Created);
            })
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.PostBooksPolicy)
            .WithName("AddBook")
            .WithOpenApi()
            .WithSummary("Adds a book")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status201Created);
    }

    private static void MapOrderRelated(this RouteGroupBuilder app)
    {
        var orderGroup = app.MapGroup("order")
            .WithTags("Orders");

        orderGroup.MapGet("{id:guid}",
                async (CancellationToken token, [FromServices] IMediator mediator, [FromRoute] Guid id) =>
                {
                    var result = await mediator.Send(new GetOrderById.Query(id), token);

                    return TranslateRemoraResult(result);
                })
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetOrdersPolicy)
            .WithName("GetOrder")
            .WithOpenApi()
            .WithSummary("Returns an order by given ID")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IOrderPayload>()
            .Produces(StatusCodes.Status404NotFound);

        orderGroup.MapGet("search",
                async (CancellationToken token, [FromServices] IMediator mediator, [AsParameters] GridifyQuery query) =>
                {
                    var result = await mediator.Send(new GetOrdersByGridifyQuery.Query(query), token);

                    return TranslateRemoraResult(result);
                })
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetOrdersPolicy)
            .WithName("SearchOrders")
            .WithOpenApi()
            .WithSummary("Searches for orders based on given parameters")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IExtendedPaging<IBookPayload>>();
    }
    
    private static void MapAuthorRelated(this RouteGroupBuilder app)
    {
        var authorGroup = app.MapGroup("author")
            .WithTags("Authors");
        
        authorGroup.MapGet("{id:long}",
                async (CancellationToken token, [FromServices] IMediator mediator, [FromRoute] long id) =>
                {
                    var result = await mediator.Send(new GetAuthorById.Query(id), token);

                    return TranslateRemoraResult(result);
                })
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetAuthorsPolicy)
            .WithName("GetAuthor")
            .WithOpenApi()
            .WithSummary("Returns an author by given ID")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IAuthorPayload>()
            .Produces(StatusCodes.Status404NotFound);

        authorGroup.MapGet("search",
                async (CancellationToken token, [FromServices] IMediator mediator, [AsParameters] GridifyQuery query) =>
                {
                    var result = await mediator.Send(new GetAuthorsByGridifyQuery.Query(query), token);

                    return TranslateRemoraResult(result);
                })
            .MapToApiVersion(1)
            .RequireAuthorization(AuthPolicies.GetAuthorsPolicy)
            .WithName("SearchAuthors")
            .WithOpenApi()
            .WithSummary("Searches for authors based on given parameters")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<IExtendedPaging<IAuthorPayload>>();
    }
    
    public static WebApplication MapBookLibrary(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var baseGroup = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet)
            .RequireCors(AuthPolicies.CorsPolicy);
        
        baseGroup.MapHealthChecks("/health", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }).WithTags("Health");
        
        baseGroup.MapOrderRelated();

        baseGroup.MapInventoryRelated();
        
        baseGroup.MapAuthorRelated();

        return app;
    }
}