using Domain.Base;

namespace Domain;

public sealed class Order : EntityBase<Guid>
{
    private HashSet<OrderDetail>? _orderDetails;
    private HashSet<Publication>? _publications;

    public IEnumerable<OrderDetail>? OrderDetails => _orderDetails?.AsEnumerable();
    public IEnumerable<Publication>? Publications => _publications?.AsEnumerable();

    public void WithDetails(IEnumerable<OrderDetail> orderDetails)
    {
        ConcatDetails(orderDetails);
    }

    public void AddDetail(OrderDetail orderDetail)
    {
        ConcatDetails([ orderDetail ]);
    }
    
    
    public void RemoveDetail(OrderDetail orderDetail)
    {
        _orderDetails?.Remove(orderDetail);
    }

    private void ConcatDetails(IEnumerable<OrderDetail> orderDetails)
    {
        var array = orderDetails as OrderDetail[] ?? orderDetails.ToArray();
        
        if (_orderDetails is null)
        {
            _orderDetails = array.ToHashSet();
            return;
        }

        var uniqueDetailGroups = array.GroupBy(x => new { x.PublicationId, x.Publication?.Type });

        foreach (var bookGroup in uniqueDetailGroups)
        {
            var count = bookGroup.Count();
            
            if (count <= 1)
            {
                continue;
            }
            
            // keep original and increment, remove newly created
            var oldest = bookGroup.MinBy(x => x.CreatedAt);

            if (oldest is null)
            {
                // maybe some logging etc.
                continue;
            }
                
            var quantitySum = bookGroup.Sum(x => x.Quantity);
                
            oldest.Quantity = quantitySum;
            
            var removed = _orderDetails.RemoveWhere(x => oldest.PublicationId != x.PublicationId);

            if (removed != count - 1)
            {
                // maybe some logging etc.
                continue;
            }
        }
    }
}