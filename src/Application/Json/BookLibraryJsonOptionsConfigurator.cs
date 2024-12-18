using System.Text.Json;
using Microsoft.Extensions.Options;
using Models.Author;
using Models.Book;
using Models.Order;
using Remora.Rest.Extensions;

namespace Application.Json;

public class BookLibraryJsonOptionsConfigurator : IConfigureNamedOptions<JsonSerializerOptions>
{
    public const string Name = "BookLibraryJsonOptions";
    
    public void Configure(JsonSerializerOptions options)
    {
        ConfigurePrivate(options);
    }

    public void Configure(string? name, JsonSerializerOptions options)
    {
        if (name != Name)
        {
            return;
        }
        
        ConfigurePrivate(options);
    }

    private void ConfigurePrivate(JsonSerializerOptions options)
    {
        options.AddDataObjectConverter<IAddBookPayload, AddBookPayload>();
        options.AddDataObjectConverter<IBookPayload, BookPayload>();

        options.AddDataObjectConverter<IAuthorPayload, AuthorPayload>();

        options.AddDataObjectConverter<IOrderPayload, OrderPayload>();
        options.AddDataObjectConverter<IOrderDetailPayload, OrderDetailPayload>();
    }
}