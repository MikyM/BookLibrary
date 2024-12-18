namespace Models.Order;

public record OrderPayload(Guid Id, IEnumerable<OrderDetailPayload> Details) : IOrderPayload;