using AutoMapper;
using FluentValidation;
using MassTransit;
using RecordHub.BasketService.Applicatation.Exceptions;
using RecordHub.BasketService.Applicatation.Services;
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
        private readonly IValidator<ShoppingCartItem> _validatorItem;
        private readonly IValidator<ShoppingCart> _validatorCart;
        private readonly IMapper _mapper;
        private readonly CatalogChecker.CatalogCheckerClient _catalogCheckerClient;
        public BasketService(IBasketRepository repo,
            IValidator<ShoppingCartItem> validatorItem,
            IValidator<ShoppingCart> validatorCart,
            CatalogChecker.CatalogCheckerClient catalogCheckerClient,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _publishEndpoint = publishEndpoint;
            _validatorItem = validatorItem;
            _validatorCart = validatorCart;
            _catalogCheckerClient = catalogCheckerClient;
            _mapper = mapper;
        }

        public Task<bool> ClearBasketAsync(string? userName, CancellationToken cancellationToken = default)
        {
            return _repo.ClearBasketAsync(userName);
        }


        public async Task<ShoppingCart?> GetBasketAsync(string? userId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetBasketAsync(userId);
        }

        public async Task RemoveCartItemAsync(string? userName, string productId, CancellationToken cancellationToken = default)
        {
            var basket = await _repo.GetBasketAsync(userName);

            if (basket == null)
            {
                throw new BasketIsEmptyException();
            }

            var itemToDelete = basket.Items.Find(i => i.ProductId.Equals(productId));
            if (itemToDelete == null)
            {
                throw new ItemMissingInBasketException();
            }

            basket.Items.Remove(itemToDelete);

            await _repo.UpdateBasket(basket);
        }

        public async Task UpdateCartItemAsync(string? userName, ShoppingCartItemModel model, CancellationToken cancellationToken = default)
        {
            var basket = await _repo.GetBasketAsync(userName) ?? new ShoppingCart
            {
                UserName = userName,
                Items = new List<ShoppingCartItem>()
            };

            var reply = await CheckProductExistenceAsync(model.ProductId);

            ShoppingCartItem item = new ShoppingCartItem
            {
                Price = reply.Price,
                Quantity = model.Quantity,
                ProductName = reply.Name,
                ProductId = model.ProductId,
            };

            await ValidateShoppingCartItemAsync(item, cancellationToken);

            await ValidateShoppingCartAsync(basket, cancellationToken);

            var oldItemIndex = basket.Items.FindIndex(i => i.ProductId == model.ProductId);
            if (oldItemIndex == -1)
            {
                basket.Items.Add(item);
            }
            else
            {
                basket.Items[oldItemIndex] = item;
            }

            await _repo.UpdateBasket(basket);
        }

        private async Task<ProductReply> CheckProductExistenceAsync(string productId, CancellationToken cancellationToken = default)
        {
            var reply = await _catalogCheckerClient.CheckProductExistingAsync(new ProductRequest { ProductId = productId });
            if (!reply.IsExisting)
            {
                throw new EntityNotFoundException(nameof(productId));
            }

            return reply;
        }

        private async Task ValidateShoppingCartItemAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
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

        private async Task ValidateShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
        {
            var validationResults = await _validatorCart.ValidateAsync(cart);
            if (!validationResults.IsValid)
            {
                throw new InvalidRequestBodyException
                {
                    Errors = validationResults.Errors.Select(e => e.ErrorMessage)
                };
            }
        }

        public async Task CheckoutAsync(BasketCheckoutModel model, CancellationToken cancellationToken = default)
        {
            var basket = await _repo.GetBasketAsync(model.UserId);

            if (basket == null)
            {
                throw new BasketIsEmptyException();
            }

            var checkoutMessage = _mapper.Map<BasketCheckoutMessage>(model);
            checkoutMessage.TotalPrice = basket.TotalPrice;
            checkoutMessage.Items = _mapper.Map<IEnumerable<OrderItemModel>>(basket.Items);

            await _publishEndpoint.Publish(checkoutMessage);

            await _repo.ClearBasketAsync(model.UserId);
        }
    }
}
