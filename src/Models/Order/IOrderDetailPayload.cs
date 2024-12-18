namespace Models.Order;

public interface IOrderDetailPayload
{
    public long PublicationId { get; }
    public long Quantity { get; }
}