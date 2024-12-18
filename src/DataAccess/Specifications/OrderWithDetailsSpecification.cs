using Domain;
using Models.Order;

namespace DataAccess.Specifications.Book;

public class OrderWithDetailsSpecification : EntityProjectedSpecification<Order,Guid,OrderPayload>
{
    public OrderWithDetailsSpecification(Guid id) : base(id)
    {
        Configure();
    }

    public OrderWithDetailsSpecification()
    {
        Configure();
    }

    private void Configure()
    {
        Include(x => x.OrderDetails);
    }
}