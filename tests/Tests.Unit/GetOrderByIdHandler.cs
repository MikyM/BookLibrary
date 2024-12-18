using Application.MediatR.Queries.Order;
using DataAccess;
using DataExplorer.Abstractions.Specifications;
using DataExplorer.EfCore.Abstractions.DataServices;
using Domain;
using FluentAssertions;
using JetBrains.Annotations;
using Models.Order;
using Moq;

namespace Tests.Unit;

public static class GetOrderByIdHandler
{
    [UsedImplicitly]
    public class Fixture
    {
        public Mock<IReadOnlyDataService<Order, Guid, IBookLibraryDbContext>> DataServiceMock => new(MockBehavior.Loose);
        
        public GetOrderById.Handler Sut(IReadOnlyDataService<Order, Guid, IBookLibraryDbContext> service) => new(service);
    }
    
    public class HandleShould(Fixture fixture) : IClassFixture<Fixture>
    {
        // oversimplified to a single test for a single path
        [Fact]
        public async Task CallServicesAccordingly()
        {
            // Arrange
            
            var id = Guid.NewGuid();
            
            var query = new GetOrderById.Query(id);

            var ds = fixture.DataServiceMock;
            
            ds.Setup(x => x.GetSingleAsync(It.IsAny<ISpecification<Order,OrderPayload>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OrderPayload(id, new List<OrderDetailPayload>()));
  
            var sut = fixture.Sut(ds.Object);
                
            // Act
            
            var result = await sut.Handle(query, CancellationToken.None);
            
            // Assert
            
            result.IsDefined(out var returned).Should().BeTrue();
            returned!.Id.Should().Be(id);
        } 
    }
}