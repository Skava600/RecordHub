using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MassTransit;
using RecordHub.BasketService.Application.Exceptions;
using RecordHub.BasketService.Application.Protos;
using RecordHub.BasketService.Application.Services;
using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Domain.Models;
using RecordHub.BasketService.Infrastructure.Data.Repositories;
using RecordHub.BasketService.Tests.Generators;
using RecordHub.BasketService.Tests.Setups;
using RecordHub.Shared.Exceptions;
using RecordHub.Shared.MassTransit.Models.Order;
using ValidationResult = FluentValidation.Results.ValidationResult;
namespace RecordHub.BasketService.Tests.UnitTests.Services
{
    public class BasketServiceTests
    {
        private readonly Infrastructure.Services.BasketService _basketService;
        private readonly Mock<IBasketRepository> _basketRepositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IValidator<BasketItem>> _validatorItemMock;
        private readonly Mock<IValidator<Basket>> _validatorCartMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICatalogGrpcClient> _catalogGrpcClientMock;

        public BasketServiceTests()
        {
            _basketRepositoryMock = new Mock<IBasketRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _validatorItemMock = new Mock<IValidator<BasketItem>>();
            _validatorCartMock = new Mock<IValidator<Basket>>();
            _mapperMock = new Mock<IMapper>();
            _catalogGrpcClientMock = new Mock<ICatalogGrpcClient>();

            _basketService = new Infrastructure.Services.BasketService(
                _basketRepositoryMock.Object,
                _validatorItemMock.Object,
                _validatorCartMock.Object,
                _catalogGrpcClientMock.Object,
                _mapperMock.Object,
                _publishEndpointMock.Object
            );
        }

        [Fact]
        public async Task ClearBasketAsync_ValidUserName_ReturnsTrue()
        {
            // Arrange
            string userName = "testuser";
            _basketRepositoryMock.SetupClearBasketAsync(userName, true);

            // Act
            var result = await _basketService.ClearBasketAsync(userName);

            // Assert
            _basketRepositoryMock.Verify();
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ClearBasketAsync_NotValidUserName_ReturnsTrue()
        {
            // Arrange
            string userName = "not-valid-user";
            _basketRepositoryMock.SetupClearBasketAsync(userName, false);

            // Act
            var result = await _basketService.ClearBasketAsync(userName);

            // Assert
            _basketRepositoryMock.Verify();
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveBasketItemAsync_ValidData_RemovesItemFromBasket()
        {
            // Arrange
            string userName = "testuser";
            var item = BasketItemGenerator.Generate();
            var basket = new Basket(userName);

            basket.UpdateItem(item);
            basket.UpdateItem(BasketItemGenerator.Generate());

            _basketRepositoryMock.SetupGetBasketAsync(userName, basket);

            // Act
            await _basketService.RemoveBasketItemAsync(userName, item.ProductId);

            // Assert
            basket.Items.Should().HaveCount(1);
            basket.Items.Should().NotContain(i => i.ProductId == item.ProductId);
            _basketRepositoryMock.Verify(m => m.UpdateBasket(It.IsAny<Basket>()), Times.Once);
        }

        [Fact]
        public async Task RemoveBasketItemAsync_BasketNotFound_ThrowsBasketIsEmptyException()
        {
            // Arrange
            string userName = "testuser";
            string productId = "product123";

            _basketRepositoryMock.SetupGetBasketAsync(userName, null);

            // Act and Assert
            await FluentActions.Awaiting(() => _basketService.RemoveBasketItemAsync(userName, productId))
                .Should().ThrowAsync<BasketIsEmptyException>();
        }

        [Fact]
        public async Task RemoveBasketItemAsync_ItemNotFound_ThrowsItemMissingInBasketException()
        {
            // Arrange
            string userName = "testuser";
            string productId = "product123";

            var basket = new Basket(userName);
            basket.UpdateItem(BasketItemGenerator.Generate());

            _basketRepositoryMock.SetupGetBasketAsync(userName, basket);

            // Act and Assert
            await FluentActions.Awaiting(() => _basketService.RemoveBasketItemAsync(userName, productId))
                .Should().ThrowAsync<ItemMissingInBasketException>();
        }

        [Fact]
        public async Task UpdateBasketItemAsync_ValidProduct_UpdateBasketWithNewItem()
        {
            // Arrange
            var userName = "testuser";
            var productId = "product1";
            var model = new BasketItemModel
            {
                ProductId = productId,
                Quantity = 3,
            };
            var basket = new Basket(userName);

            _basketRepositoryMock.SetupGetBasketAsync(userName, basket);
            _catalogGrpcClientMock.SetupCheckProductExistenceAsync(productId, new ProductReply
            {
                Name = "record",
                Price = 100,
                IsExisting = true
            });

            _validatorItemMock.SetupValidatorMock(new ValidationResult());
            _validatorCartMock.SetupValidatorMock(new ValidationResult());

            // Act
            await _basketService.UpdateBasketItemAsync(userName, model);

            // Assert
            basket.Items.Should().ContainSingle(item =>
                item.ProductId == productId && item.Price == 100 && item.Quantity == model.Quantity);
        }

        [Fact]
        public async Task UpdateBasketItemAsync_NotExistingProduct_ThrowsInvalidRequestBodyException()
        {
            // Arrange
            var userName = "testuser";
            var productId = "nonexistentproduct";
            var model = new BasketItemModel
            {
                ProductId = productId,
                Quantity = 3,
            };
            var basket = new Basket(userName);

            _basketRepositoryMock.SetupGetBasketAsync(userName, basket);
            _catalogGrpcClientMock.SetupCheckProductExistenceAsync(productId, new ProductReply { IsExisting = false });

            // Act and Assert
            await FluentActions
                .Awaiting(() => _basketService.UpdateBasketItemAsync(userName, model))
                .Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task CheckoutAsync_ValidBasket_CheckoutAndClearBasket()
        {
            // Arrange
            var model = BasketCheckoutModelGenerator.GenerateModel();

            var basket = new Basket(model.UserId);
            var items = BasketItemGenerator.GenerateBeetween(1, 10);
            foreach (var item in items)
            {
                basket.UpdateItem(item);
            }

            _basketRepositoryMock.SetupGetBasketAsync(model.UserId, basket);
            _mapperMock.SetupMap(model, new BasketCheckoutMessage());
            // Act
            await _basketService.CheckoutAsync(model);

            // Assert
            _mapperMock.Verify();
            _basketRepositoryMock.Verify(repo => repo.ClearBasketAsync(model.UserId), Times.Once());
        }

        [Fact]
        public async Task CheckoutAsync_EmptyBasket_ThrowsBasketIsEmptyException()
        {
            // Arrange
            var model = BasketCheckoutModelGenerator.GenerateModel();

            _basketRepositoryMock.SetupGetBasketAsync(model.UserId, null);

            // Act and Assert
            await FluentActions
                .Awaiting(() => _basketService.CheckoutAsync(model))
                .Should().ThrowAsync<BasketIsEmptyException>();
        }

    }
}

