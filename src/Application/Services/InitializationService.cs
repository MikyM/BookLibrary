using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class InitializationService : IHostedService
{
    private readonly ILogger<InitializationService> _logger;
    private readonly IBookLibraryDbContext _dbContext;

    public InitializationService(ILogger<InitializationService> logger, IBookLibraryDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing the application..");

        var applied = await _dbContext.Database.GetAppliedMigrationsAsync(cancellationToken);

        await _dbContext.Database.MigrateAsync(cancellationToken);

        // treat no migrations applied as a fresh db
        if (!applied.Any())
        {
            await SeedAsync(cancellationToken);
        }

        /*var allBooks = await _dbContext.Set<Book>().Include(x => x.Authors).ToListAsync(cancellationToken: cancellationToken);
        var allMagazines = await _dbContext.Set<Magazine>().Include(x => x.Authors).ToListAsync(cancellationToken: cancellationToken);
        var allPublications = await _dbContext.Set<Publication>().Include(x => x.Authors).ToListAsync(cancellationToken: cancellationToken);
        
        var allAuthors = await _dbContext.Set<Author>().Include(x => x.Publications).ToListAsync(cancellationToken: cancellationToken);*/
        
        _logger.LogInformation("Application initialized");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping the application..");
        
        return Task.CompletedTask;
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding the database..");

        var author1 = new Author() { FirstName = "John", LastName = "Doe" };
        var author2 = new Author() { FirstName = "Jane", LastName = "Doe" };
        var author3 = new Author() { FirstName = "Julie", LastName = "Doe" };
        var author4 = new Author() { FirstName = "Julian", LastName = "Doe" };
        
        await _dbContext.AddRangeAsync(author1, author2, author3, author4);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var book1 = new Book() { Title = "Book 1", Bookstand = 1, Shelf = 1, Price = 10.56m };
        book1.WithAuthors(new List<Author> { author1, author3 });
        
        var book2 = new Book() { Title = "Book 2", Bookstand = 1, Shelf = 2, Price = 23m };
        book2.WithAuthor(author1);
        
        var book3 = new Book() { Title = "Book 3", Bookstand = 2, Shelf = 1, Price = 50.99m };
        book3.WithAuthor(author1);
        book3.WithAuthor(author4);
        
        await _dbContext.AddRangeAsync(book1, book2, book3);
            
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var magazine1 = new Magazine() { Title = "Magazine 1", Bookstand = 3, Shelf = 1, Price = 15.99m };
        magazine1.WithAuthors(new List<Author> { author1, author2 });
        
        var magazine2 = new Magazine() { Title = "Magazine 2", Bookstand = 3, Shelf = 1, Price = 77m };
        magazine2.WithAuthor(author2);
        
        await _dbContext.AddRangeAsync(magazine1, magazine2);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        var order1 = new Order();
        var order2 = new Order();
        
        order1.AddDetail(book1, 1);
        order1.AddDetail(book2, 5);
        
        order2.AddDetail(book1, 1);
        order2.AddDetail(magazine1, 3);
        order2.AddDetail(magazine2, 3);
        
        await _dbContext.AddRangeAsync(order1, order2);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}