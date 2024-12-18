namespace Models.Order;

public record OrderDetailPayload(long PublicationId, long Quantity) : IOrderDetailPayload;