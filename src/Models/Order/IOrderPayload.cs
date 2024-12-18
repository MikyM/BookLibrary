namespace Models.Order;

public interface IOrderPayload
{
    public Guid Id { get; }
    public IEnumerable<OrderDetailPayload> Details { get; }
}