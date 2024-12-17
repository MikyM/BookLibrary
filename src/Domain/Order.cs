using Domain.Base;

namespace Domain;

public sealed class Order : EntityBase<Guid>
{
    private HashSet<OrderDetail>? _orderDetails;
    private HashSet<Publication>? _publications;

    public override Guid Id { get; protected set; } = Guid.NewGuid();

    public IEnumerable<OrderDetail>? OrderDetails => _orderDetails?.AsEnumerable();
    public IEnumerable<Publication>? Publications => _publications?.AsEnumerable();
    
    public void AddDetail(Publication publication, int quantity)
    {
        var detail = new OrderDetail(Id, publication.Id, quantity);
        
        ConcatDetails([ detail ]);
    }

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
        var preMerged = _orderDetails is null
            ? orderDetails
            : _orderDetails.Concat(orderDetails);

        var uniqueDetailGroups = preMerged.GroupBy(x => x.PublicationId);

        foreach (var publicationGroup in uniqueDetailGroups)
        {
            var quantity = publicationGroup.Sum(x => x.Quantity);
            
            if (_orderDetails is not null)
            {
                var existing = _orderDetails.FirstOrDefault(x => x.PublicationId == publicationGroup.Key);
                
                if (existing is not null)
                {
                    existing.Quantity = quantity;
                }
                else
                {
                    var toAdd = publicationGroup.First();
                    toAdd.Quantity = quantity;

                    _orderDetails.Add(toAdd);
                }
            }
            else
            {
                _orderDetails = new HashSet<OrderDetail>();
                
                var toAdd = publicationGroup.First();
                toAdd.Quantity = quantity;
                
                _orderDetails.Add(toAdd);
            }
        }
    }
}