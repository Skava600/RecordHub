using AutoMapper;
using RecordHub.OrderingService.Domain.Entities;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.OrderingService.Application.Mappers
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<BasketCheckoutMessage, Order>();

            CreateMap<OrderItemModel, OrderItem>();
        }
    }
}
