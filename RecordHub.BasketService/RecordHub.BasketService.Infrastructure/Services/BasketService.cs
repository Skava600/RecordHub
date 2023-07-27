using AutoMapper;
using FluentValidation;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using RecordHub.BasketService.Application.Exceptions;
using RecordHub.BasketService.Application.Services;
using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Domain.Models;
using RecordHub.BasketService.Infrastructure.Data.Repositories;
using RecordHub.Shared.Exceptions;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.BasketService.Infrastructure.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _repo;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IValidator<BasketItem> _validatorItem;
        private readonly IValidator<Basket> _validatorCart;
        private readonly IMapper _mapper;
        private readonly ICatalogGrpcClient _catalogGrpcClient;

        public BasketService(
            IBasketRepository repo,
            IValidator<BasketItem> validatorItem,
            IValidator<Basket> validatorCart,
            ICatalogGrpcClient catalogCheckerClient,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _publishEndpoint = publishEndpoint;
            _validatorItem = validatorItem;
            _validatorCart = validatorCart;
            _catalogGrpcClient = catalogCheckerClient;
            _mapper = mapper;
        }

        public Task<bool> ClearBasketAsync(string? userName, CancellationToken cancellationToken = default)
        {
            return _repo.ClearBasketAsync(userName);
        }

        public async Task<Basket?> GetBasketAsync(string? userId, CancellationToken cancellationToken = default)
        {
            var basketItems = await _repo.GetBasketAsync(userId);
            var basket = new Basket(userId, basketItems);

            return basket;
        }

        public async Task RemoveBasketItemAsync(
            string? userName,
            string productId,
            CancellationToken cancellationToken = default)
        {
            var basketItems = await _repo.GetBasketAsync(userName);
            if (basketItems == null)
            {
                throw new BasketIsEmptyException();
            }

            var basket = new Basket(userName, basketItems.ToList());
            var itemToDelete = basket.Items.FirstOrDefault(i => i.ProductId.Equals(productId));
            if (itemToDelete == null)
            {
                throw new ItemMissingInBasketException();
            }

            basket.RemoveItem(itemToDelete);

            await _repo.UpdateBasket(basket.UserName, basket.Items);
        }

        public async Task UpdateBasketItemAsync(
            string? userName,
            BasketItemModel model,
            CancellationToken cancellationToken = default)
        {
            var basketItems = await _repo.GetBasketAsync(userName) ?? Enumerable.Empty<BasketItem>();

            var basket = new Basket(userName, basketItems.ToList());
            var reply = await _catalogGrpcClient.CheckProductExistenceAsync(model.ProductId);

            if (!reply.IsExisting)
            {
                throw new EntityNotFoundException(nameof(model.ProductId));
            }

            BasketItem item = new BasketItem
            {
                Price = reply.Price,
                Quantity = model.Quantity,
                ProductName = reply.Name,
                ProductId = model.ProductId,
            };

            await ValidateShoppingCartItemAsync(item, cancellationToken);

            await ValidateShoppingCartAsync(basket, cancellationToken);

            basket.UpdateItem(item);
            await _repo.UpdateBasket(basket.UserName, basket.Items);
        }

        public async Task CheckoutAsync(
            BasketCheckoutModel model,
            CancellationToken cancellationToken = default)
        {
            var basketItems = await _repo.GetBasketAsync(model.UserId);

            var basket = new Basket(model.UserId, basketItems);
            if (basket.Items.IsNullOrEmpty())
            {
                throw new BasketIsEmptyException();
            }

            var checkoutMessage = _mapper.Map<BasketCheckoutMessage>(model);
            checkoutMessage.TotalPrice = basket.TotalPrice;
            checkoutMessage.Items = _mapper.Map<IEnumerable<OrderItemModel>>(basket.Items);

            await _publishEndpoint.Publish(checkoutMessage);

            await _repo.ClearBasketAsync(model.UserId);
        }

        private async Task ValidateShoppingCartItemAsync(
            BasketItem item,
            CancellationToken cancellationToken = default)
        {
            var validationResults = await _validatorItem.ValidateAsync(item, cancellationToken);
            if (!validationResults.IsValid)
            {
                throw new InvalidRequestBodyException
                {
                    Errors = validationResults.Errors.Select(e => e.ErrorMessage)
                };
            }
        }

        private async Task ValidateShoppingCartAsync(
            Basket cart,
            CancellationToken cancellationToken = default)
        {
            var validationResults = await _validatorCart.ValidateAsync(cart, cancellationToken);
            if (!validationResults.IsValid)
            {
                throw new InvalidRequestBodyException
                {
                    Errors = validationResults.Errors.Select(e => e.ErrorMessage)
                };
            }
        }
    }
}
