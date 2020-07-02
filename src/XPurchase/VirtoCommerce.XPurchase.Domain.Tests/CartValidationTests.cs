using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Validators;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests
{
    public class CartValidationTests : MoqHelper
    {
        private const string CART_NAME = "default";
        private const string CURRENCY_CODE = "USD";
        private const int InStockQuantity = 100;
        private const decimal MIN_PRICE = 1;
        private const decimal MAX_PRICE = 50;
        private static readonly string[] ShipmentMehodCodes = new[] { "FedEx", "DHL", "EMS" };
        private static readonly Randomizer Rand = new Randomizer();
        private static readonly Faker Faker = new Faker();

        [Fact]
        public async Task ValidateCart_RuleSetDefault_Valid()
        {
            // Arrange
            var validator = new CartValidator();
            var aggregate = await GetValidCartAggregateAsync();

            // Act
            var result = await validator.ValidateAsync(aggregate, ruleSet: "default");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Empty(aggregate.ValidationErrors);
        }

        [Fact]
        public async Task ValidateCart_RuleSetDefault_Invalid()
        {
            // Arrange

            var validator = new CartValidator();
            var aggregate = await GetValidCartAggregateAsync();
            aggregate.Cart.Name = null;
            aggregate.Cart.CustomerId = null;

            // Act
            var result = await validator.ValidateAsync(aggregate, ruleSet: "default");

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(aggregate.ValidationErrors);
            Assert.NotEmpty(result.Errors);
            Assert.Equal(4, result.Errors.Count);

            Assert.Collection(result.Errors, x => { Assert.Equal(nameof(aggregate.Cart.Name), x.PropertyName); Assert.Equal(nameof(NotNullValidator), x.ErrorCode); }
                                           , x => { Assert.Equal(nameof(aggregate.Cart.Name), x.PropertyName); Assert.Equal(nameof(NotEmptyValidator), x.ErrorCode); }
                                           , x => { Assert.Equal(nameof(aggregate.Cart.CustomerId), x.PropertyName); Assert.Equal(nameof(NotNullValidator), x.ErrorCode); }
                                           , x => { Assert.Equal(nameof(aggregate.Cart.CustomerId), x.PropertyName); Assert.Equal(nameof(NotEmptyValidator), x.ErrorCode); }
                              );
        }

        [Fact]
        public async Task ValidateShipment_RuleSetStrict_Valid()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();

            // Act
            var validator = new CartShipmentValidator(aggregate);
            var shipmentForValidation = aggregate.Cart.Shipments.ToList()[0];
            var result = await validator.ValidateAsync(shipmentForValidation, ruleSet: "strict");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateShipment_RuleSetStrict_UnavailableMethodError()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();

            var testShipments = new Faker<Shipment>()
               .CustomInstantiator(f => new Shipment())
               .RuleFor(s => s.ShipmentMethodCode, f => f.Random.Guid().ToString());

            var unavailableShipment = testShipments.Generate();
            aggregate.Cart.Shipments.Add(unavailableShipment);

            // Act
            var validator = new CartShipmentValidator(aggregate);
            var result = await validator.ValidateAsync(unavailableShipment, ruleSet: "strict");

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);

            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal(nameof(unavailableShipment.ShipmentMethodCode), x.PropertyName);
            });
        }

        [Fact]
        public async Task ValidateShipment_RuleSetStrict_PriceError()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();
            var shipment = Faker.PickRandom(aggregate.Cart.Shipments);
            shipment.Price += 1m;
            aggregate.Cart.Shipments.Add(shipment);

            // Act
            var validator = new CartShipmentValidator(aggregate);
            var result = await validator.ValidateAsync(shipment, ruleSet: "strict");

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);

            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal(nameof(shipment.Price), x.PropertyName);
            });
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetDefault_Valid()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();
            var item = Faker.PickRandom(aggregate.Cart.Items);
            var newItemPrice = new PriceAdjustment(item.Id, Rand.Decimal(MIN_PRICE, MAX_PRICE));
            var validator = new ChangeCartItemPriceValidator(aggregate);

            // Act
            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "default");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetDefault_Invalid()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();

            var newItemPrice = new PriceAdjustment(null, -1);

            // Act
            var validator = new ChangeCartItemPriceValidator(aggregate);

            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "default");
            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal(3, result.Errors.Count);

            Assert.Collection(result.Errors, x => { Assert.Equal(nameof(newItemPrice.NewPrice), x.PropertyName); Assert.Equal(nameof(GreaterThanOrEqualValidator), x.ErrorCode); }
                                          , x => { Assert.Equal(nameof(newItemPrice.LineItemId), x.PropertyName); Assert.Equal(nameof(NotNullValidator), x.ErrorCode); }
                                          , x => { Assert.Equal(nameof(newItemPrice.LineItemId), x.PropertyName); Assert.Equal(nameof(NotEmptyValidator), x.ErrorCode); }
                             );
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetStrict_Valid()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();
            var item = Faker.PickRandom(aggregate.Cart.Items);
            var newItemPrice = new PriceAdjustment(item.Id, item.ListPrice + Rand.Decimal(MIN_PRICE, MAX_PRICE));
            var validator = new ChangeCartItemPriceValidator(aggregate);

            // Act
            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "strict");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetStrict_Invalid()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();
            var item = Faker.PickRandom(aggregate.Cart.Items);
            var newItemPrice = new PriceAdjustment(item.Id, item.ListPrice - Rand.Decimal(0, item.ListPrice));
            var validator = new ChangeCartItemPriceValidator(aggregate);

            // Act
            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "strict");

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal(nameof(item.SalePrice), x.PropertyName);
            });
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetDefault_Valid()
        {
            // Arrange
            var validator = new NewCartItemValidator();
            var newCartItem = new NewCartItem(Rand.Guid().ToString(), Rand.Int(1, InStockQuantity));

            // Act
            var result = await validator.ValidateAsync(newCartItem, ruleSet: "default");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetDefault_Invalid()
        {
            // Arrange
            var validator = new NewCartItemValidator();
            var newCartItem = new NewCartItem(Rand.Guid().ToString(), 0);

            // Act
            var result = await validator.ValidateAsync(newCartItem, ruleSet: "default");

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Collection(result.Errors,
                x =>
                {
                    Assert.Equal(nameof(newCartItem.Quantity), x.PropertyName);
                    Assert.Equal(nameof(GreaterThanValidator), x.ErrorCode);
                },
                x =>
                {
                    Assert.Equal(nameof(newCartItem.ProductId), x.PropertyName);
                    Assert.Equal(nameof(NotNullValidator), x.ErrorCode);
                }
            );
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetStrict_Valid()
        {
            // Arrange
            var productPrice = Rand.Decimal(MIN_PRICE, MAX_PRICE);
            var newCartItem = new NewCartItem(Rand.Guid().ToString(), Rand.Int(1, InStockQuantity))
            {
                Price = productPrice
            };
            var validator = new NewCartItemValidator();

            // Act
            var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetStrict_PriceError()
        {
            // Arrange
            var productPrice = Rand.Decimal(MIN_PRICE, Rand.Decimal(MIN_PRICE, MAX_PRICE) - 1);
            var productId = Rand.Guid().ToString();
            var quantity = Rand.Int(1, InStockQuantity);
            var newCartItem = BuildNewCartItem(productId, quantity, productPrice);
            var validator = new NewCartItemValidator();

            // Act
            var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal(nameof(newCartItem.Price), x.PropertyName);
            });
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetStrict_UnavailableQuantity()
        {
            // Arrange
            var validator = new NewCartItemValidator();
            var productPrice = Rand.Decimal(MIN_PRICE, MAX_PRICE);
            var productId = Rand.Guid().ToString();
            var quantity = Rand.Int(InStockQuantity + 1, InStockQuantity * 2);
            var newCartItem = BuildNewCartItem(productId, quantity, productPrice);

            // Act
            var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Collection(result.Errors, x => { Assert.Equal(nameof(newCartItem.ProductId), x.PropertyName); Assert.Equal(nameof(PredicateValidator), x.ErrorCode); });
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_Valid()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();
            var item = Faker.PickRandom(aggregate.Cart.Items);

            // Act
            var validator = new CartLineItemValidator(aggregate);
            var result = await validator.ValidateAsync(item, ruleSet: "strict");

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("Product_null")]
        [InlineData("Product_IsActive_false")]
        [InlineData("Product_IsBuyable_false")]
        public async Task ValidateCartLineItem_RuleSetStrict_UnavailableError(string scenario)
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();
            var cartProduct = Faker.PickRandom(aggregate.CartProductsDict["defaultProduct"]);
            var lineItem = new LineItem { ProductId = cartProduct.Id };
            switch (scenario)
            {
                case "Product_null":
                    cartProduct = null;
                    break;

                case "Product_IsActive_false":
                    cartProduct.Product.IsActive = false;
                    break;

                case "Product_IsBuyable_false":
                    cartProduct.Product.IsBuyable = false;
                    break;
            }

            // Act
            var validator = new CartLineItemValidator(aggregate);
            var result = await validator.ValidateAsync(lineItem, ruleSet: "strict");

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Collection(result.Errors, x => { Assert.Equal(nameof(lineItem.ProductId), x.PropertyName); });
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_QuantityError()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();
            var item = Faker.PickRandom(aggregate.Cart.Items);

            item.Quantity = InStockQuantity * 2;

            // Act
            var validator = new CartLineItemValidator(aggregate);
            var result = await validator.ValidateAsync(item, ruleSet: "strict");

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_PriceError()
        {
            // Arrange
            var aggregate = await GetValidCartAggregateAsync();
            var item = Faker.PickRandom(aggregate.Cart.Items);

            item.SalePrice /= 2m;

            // Act
            var validator = new CartLineItemValidator(aggregate);
            var result = await validator.ValidateAsync(item, ruleSet: "strict");

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Collection(result.Errors, x => { Assert.Equal(nameof(item.SalePrice), x.PropertyName); });
        }

        private async Task<CartAggregate> GetValidCartAggregateAsync()
        {
            var testItems = new Faker<LineItem>()
                .CustomInstantiator(f => new LineItem())
                .RuleFor(i => i.Id, f => f.Random.Guid().ToString())
                .RuleFor(i => i.ListPrice, f => f.Random.Decimal(MIN_PRICE, MAX_PRICE))
                .RuleFor(i => i.SalePrice, (f, i) => i.ListPrice);

            var testShipments = new Faker<Shipment>()
                .CustomInstantiator(f => new Shipment())
                .RuleFor(s => s.ShipmentMethodCode, f => f.PickRandom(ShipmentMehodCodes))
                .RuleFor(s => s.ShipmentMethodOption, f => "")
                .RuleFor(s => s.Price, f => 20);

            var cart = CreateCart();
            cart.Items = testItems.Generate(5).ToList();
            cart.Shipments = testShipments.Generate(1).ToList();
            cart.CustomerId = Guid.NewGuid().ToString();
            cart.CustomerName = Faker.Name.FullName();

            var aggregate = new CartAggregate(
                _cartProductServiceMock.Object,
                _currencyServiceMock.Object,
                _marketingPromoEvaluatorMock.Object,
                _paymentMethodsSearchServiceMock.Object,
                _shippingMethodsSearchServiceMock.Object,
                _shoppingCartTotalsCalculatorMock.Object,
                _storeServiceMock.Object,
                _taxProviderSearchServiceMock.Object,
                _mapperMock.Object);

            return await aggregate.TakeCartAsync(cart);
        }
    }
}
