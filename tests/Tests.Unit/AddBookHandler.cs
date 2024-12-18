using Application.MediatR.Commands.Book;
using AutoMapper;
using DataAccess;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.IdGenerator;
using Domain;
using FluentAssertions;
using JetBrains.Annotations;
using Models.Author;
using Models.Book;
using Moq;

namespace Tests.Unit;

public static class AddBookHandler
{
    [UsedImplicitly]
    public class Fixture
    {
        public Mock<ICrudDataService<Book, IBookLibraryDbContext>> DataServiceMock => new(MockBehavior.Loose);
        
        public AddBook.Handler Sut(ICrudDataService<Book, IBookLibraryDbContext> service) => new AddBook.Handler(service);
    }
    
    public class HandleShould(Fixture fixture) : IClassFixture<Fixture>
    {
        // oversimplified to a single test for a single path
        [Fact]
        public async Task CallServicesAccordingly()
        {
            // Arrange
            
            SnowflakeIdFactory.AddFactoryMethod(() => Random.Shared.NextInt64());
            
            var author = new AuthorPayload("John", "Doe");
            var authors = new List<AuthorPayload> { author };
            var payload = new AddBookPayload(authors, "Book 1", 2.56m, 1, 2);

            var book = new Book
            {
                Title = "Book 1",
                Price = 2.56m,
                Bookstand = 1,
                Shelf = 2
            };
            
            book.WithAuthors(payload.Authors.Select(x => new Author() { FirstName = x.FirstName, LastName = x.LastName }).ToList());
            
            var returnedPayload = new BookPayload(book.Id, book.Authors!.Select(x => new AuthorPayload(x.FirstName, x.LastName)), book.Title, book.Price, book.Bookstand, book.Shelf);

            var mapperMock = new Mock<IMapper>(MockBehavior.Loose);
            mapperMock.Setup(x => x.Map<Book>(It.Is<IAddBookPayload>(y => y.Title == payload.Title)))
                .Returns(book);
            mapperMock.Setup(x => x.Map<IBookPayload>(It.Is<Book>(y => y.Title == payload.Title)))
                .Returns(returnedPayload);

            var ds = fixture.DataServiceMock;
            ds.SetupGet(x => x.Mapper).Returns(mapperMock.Object);
            
            ds.Setup(x => x.AddAsync(It.Is<Book>(y => y.Title == payload.Title), true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            
            var sut = fixture.Sut(ds.Object);
            
            var command = new AddBook.Command(payload, true);
                
            // Act
            
            var result = await sut.Handle(command, CancellationToken.None);
            
            // Assert
            
            result.IsDefined(out var returned).Should().BeTrue();
            returned.Value.Should().BeAssignableTo<IBookPayload>();
            returned.Value.As<IBookPayload>().Title.Should().Be(payload.Title);
        } 
    }
}