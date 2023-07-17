using AutoMapper;
using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Domain.Models;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.BasketService.Application.Mappers
{
    public class CheckoutProfile : Profile
    {
        public CheckoutProfile()
        {
            CreateMap<BasketCheckoutModel, BasketCheckoutMessage>();
            CreateMap<BasketItem, OrderItemModel>();
        }
    }
}
