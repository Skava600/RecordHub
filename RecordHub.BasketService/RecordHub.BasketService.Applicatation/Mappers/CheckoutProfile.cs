using AutoMapper;
using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Domain.Models;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.BasketService.Applicatation.Mappers
{
    public class CheckoutProfile : Profile
    {
        public CheckoutProfile()
        {
            CreateMap<BasketCheckoutModel, BasketCheckoutMessage>();
            CreateMap<ShoppingCartItem, OrderItemModel>();
        }
    }
}
