using Application.MediatR.Commands.Book;
using Application.MediatR.Queries.Order;
using AutoMapper;
using AutoMapper.Internal;
using DataAccess;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.EfCore.Abstractions.Repositories;
using DataExplorer.IdGenerator;
using Domain;
using FluentAssertions;
using Gridify;
using JetBrains.Annotations;
using Models.Author;
using Models.Book;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests.Unit;

public static class GetOrdersByGridifyQueryHandler
{
    [UsedImplicitly]
    public class Fixture
    {
        public Mock<IReadOnlyDataService<Order, Guid, IBookLibraryDbContext>> DataServiceMock => new(MockBehavior.Loose);
        
        public GetOrdersByGridifyQuery.Handler Sut(IReadOnlyDataService<Order, Guid, IBookLibraryDbContext> service) => new(service);
    }
    
    public class HandleShould(Fixture fixture) : IClassFixture<Fixture>
    {
        // oversimplified to a single test for a single path
        [Fact]
        public async Task CallServicesAccordingly()
        {
            // Arrange

            var gridifyQuery = new GridifyQuery()
            {
                Page = 1,
                PageSize = 2
            };
            
            var query = new GetOrdersByGridifyQuery.Query(gridifyQuery);

            var repo = new Mock<IReadOnlyRepository<Order, Guid>>(MockBehavior.Loose);

            var context = new Mock<IBookLibraryDbContext>(MockBehavior.Loose);
            
            var mapper = new Mock<IMapper>(MockBehavior.Loose);
            var confProvider = new Mock<IConfigurationProvider>(MockBehavior.Loose)
                .As<IGlobalConfiguration>();
            
            mapper.Setup(p => p.ConfigurationProvider).Returns(confProvider.Object);

            context.Setup(x => x.Set<Order>()).ReturnsDbSet(new List<Order>());

            repo.SetupGet(x => x.Context).Returns(context.Object);
            repo.SetupGet(x => x.Set).ReturnsDbSet(new List<Order>());

            var ds = fixture.DataServiceMock;
            
            ds.SetupGet(x => x.ReadOnlyRepository).Returns(repo.Object);
            
            ds.SetupGet(x => x.Mapper).Returns(mapper.Object);
            var sut = fixture.Sut(ds.Object);
                
            // Act
            
            var result = await sut.Handle(query, CancellationToken.None);
            
            // Assert
            
            result.IsDefined(out var returned).Should().BeTrue();
            returned!.TotalPages.Should().Be(1);
            returned.Data.Should().BeEmpty();
        } 
    }
}