using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class InitializationService : IHostedService
{
    private readonly ILogger<InitializationService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InitializationService(ILogger<InitializationService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing the application..");

        await using var scope = _serviceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IBookLibraryDbContext>();

        var shouldSeed = !(await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).Any();

        await dbContext.Database.MigrateAsync(cancellationToken);

        // treat no migrations applied as a fresh db
        if (shouldSeed)
        {
            await SeedAsync(dbContext, cancellationToken);
        }

        /*var allBooks = await dbContext.Set<Book>().Include(x => x.Authors).ToListAsync(cancellationToken: cancellationToken);
        var allMagazines = await dbContext.Set<Magazine>().Include(x => x.Authors).ToListAsync(cancellationToken: cancellationToken);
        var allPublications = await dbContext.Set<Publication>().Include(x => x.Authors).ToListAsync(cancellationToken: cancellationToken);
        
        var allAuthors = await dbContext.Set<Author>().Include(x => x.Publications).ToListAsync(cancellationToken: cancellationToken);*/
        
        _logger.LogInformation("Application initialized");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping the application..");
        
        return Task.CompletedTask;
    }

    private async Task SeedAsync(IBookLibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding the database..");

        var author1 = new Author() { FirstName = "John", LastName = "Doe" };
        var author2 = new Author() { FirstName = "Jane", LastName = "Doe" };
        var author3 = new Author() { FirstName = "Julie", LastName = "Doe" };
        var author4 = new Author() { FirstName = "Julian", LastName = "Doe" };
        
        await dbContext.AddRangeAsync(author1, author2, author3, author4);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        var book1 = new Book() { Title = "Book 1", Bookstand = 1, Shelf = 1, Price = 10.56m };
        book1.WithAuthors(new List<Author> { author1, author3 });
        
        var book2 = new Book() { Title = "Book 2", Bookstand = 1, Shelf = 2, Price = 23m };
        book2.WithAuthor(author1);
        
        var book3 = new Book() { Title = "Book 3", Bookstand = 2, Shelf = 1, Price = 50.99m };
        book3.WithAuthor(author1);
        book3.WithAuthor(author4);
        
        await dbContext.AddRangeAsync(book1, book2, book3);
            
        await dbContext.SaveChangesAsync(cancellationToken);
        
        var magazine1 = new Magazine() { Title = "Magazine 1", Bookstand = 3, Shelf = 1, Price = 15.99m };
        magazine1.WithAuthors(new List<Author> { author1, author2 });
        
        var magazine2 = new Magazine() { Title = "Magazine 2", Bookstand = 3, Shelf = 1, Price = 77m };
        magazine2.WithAuthor(author2);
        
        await dbContext.AddRangeAsync(magazine1, magazine2);
        
        await dbContext.SaveChangesAsync(cancellationToken);

        var order1 = new Order();
        var order2 = new Order();
        
        order1.AddDetail(book1, 1);
        order1.AddDetail(book2, 5);
        
        order2.AddDetail(book1, 1);
        order2.AddDetail(magazine1, 3);
        order2.AddDetail(magazine2, 3);
        
        await dbContext.AddRangeAsync(order1, order2);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}