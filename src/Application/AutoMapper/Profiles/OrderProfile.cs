using AutoMapper;
using Models.Order;

namespace Application.AutoMapper.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderPayload>()
            .ConstructUsing(x => new OrderPayload(x.Id,
                x.OrderDetails!.Select(y => new OrderDetailPayload(y.PublicationId, y.Quantity))))
            .ReverseMap();

        CreateMap<Order, IOrderPayload>()
            .ConstructUsing(x => new OrderPayload(x.Id,
                x.OrderDetails!.Select(y => new OrderDetailPayload(y.PublicationId, y.Quantity))))
            .ReverseMap();

        CreateMap<OrderDetail, OrderDetailPayload>()
            .ConstructUsing(x => new OrderDetailPayload(x.PublicationId, x.Quantity))
            .ReverseMap();

        CreateMap<OrderDetail, IOrderDetailPayload>()
            .ConstructUsing(x => new OrderDetailPayload(x.PublicationId, x.Quantity))
            .ReverseMap();
    }
}