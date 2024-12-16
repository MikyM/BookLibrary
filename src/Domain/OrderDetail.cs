using DataExplorer.Abstractions.Entities;
using Domain.Base;

namespace Domain;

/// <summary>
/// Represents a join entity between any given order and any given publication.
/// </summary>
public sealed class OrderDetail : ICreatedAtOffset, IUpdatedAtOffset, IValueObject<OrderDetail>
{
    public OrderDetail(Guid orderId, long publicationId, int quantity)
    {
        PublicationId = publicationId;
        OrderId = orderId;
        Quantity = quantity;
    }
    
    public Guid OrderId { get; }
    public Order? Order { get; set; }
    
    public long PublicationId { get; }
    public Publication? Publication { get; set; }
    
    public int Quantity { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public bool Equals(OrderDetail? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return OrderId.Equals(other.OrderId) && PublicationId == other.PublicationId;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is OrderDetail other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OrderId, PublicationId);
    }
}