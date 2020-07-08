using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Validators
{
    public class CartLineItemValidatorTests : MoqHelper
    {
        private readonly CartValidationContext _context = new CartValidationContext();

        public CartLineItemValidatorTests()
        {
            _context.AllCartProducts = new List<CartProduct>()
            {
                _fixture.Create<CartProduct>()
            };
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_Valid()
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var lineItem = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = lineItem.Id;

            // Act
            var validator = new CartLineItemValidator(_context.AllCartProducts);
            var result = await validator.ValidateAsync(item, ruleSet: "strict");

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("Product_null")]
        [InlineData("Product_IsActive_false")]
        [InlineData("Product_IsBuyable_false")]
        public async Task ValidateCartLineItem_RuleSetStrict_UnavailableError(string scenario)
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var cartProduct = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = cartProduct.Id;

            switch (scenario)
            {
                case "Product_null":
                    cartProduct.Id = _fixture.Create<string>();
                    break;

                case "Product_IsActive_false":
                    cartProduct.Product.IsActive = false;
                    break;

                case "Product_IsBuyable_false":
                    cartProduct.Product.IsBuyable = false;
                    break;
            }

            // Act
            var validator = new CartLineItemValidator(_context.AllCartProducts);
            var result = await validator.ValidateAsync(item, ruleSet: "strict");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(x => x.ErrorCode == "CART_PRODUCT_UNAVAILABLE");
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_QuantityError()
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var lineItem = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = lineItem.Id;

            item.Quantity = InStockQuantity * 2;

            // Act
            var validator = new CartLineItemValidator(_context.AllCartProducts);
            var result = await validator.ValidateAsync(item, ruleSet: "strict");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_PriceError()
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var lineItem = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = lineItem.Id;

            item.SalePrice /= 2m;

            // Act
            var validator = new CartLineItemValidator(_context.AllCartProducts);
            var result = await validator.ValidateAsync(item, ruleSet: "strict");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(x => x.ErrorCode == "PRODUCT_PRICE_CHANGED");
        }
    }

    //public class CartShipmentValidatorTests : MoqHelper
    //{
    //private const decimal MIN_PRICE = 1;
    //private const decimal MAX_PRICE = 50;
    //private static readonly string[] ShipmentMehodCodes = new[] { "FedEx", "DHL", "EMS" };
    //    [Fact]
    //    public async Task ValidateShipment_RuleSetStrict_Valid()
    //    {
    //        // Arrange
    //        var aggregate = await GetValidCartAggregateAsync();

    //        // Act
    //        var validator = new CartShipmentValidator(new CartValidationContext().AvailShippingRates);
    //        var shipmentForValidation = aggregate.Cart.Shipments.ToList()[0];
    //        var result = await validator.ValidateAsync(shipmentForValidation, ruleSet: "strict");

    //        // Assert
    //        result.IsValid.Should().BeTrue();
    //        result.Errors.Should().BeEmpty();
    //    }

    //    [Fact]
    //    public async Task ValidateShipment_RuleSetStrict_UnavailableMethodError()
    //    {
    //        // Arrange
    //        var aggregate = await GetValidCartAggregateAsync();

    //        var testShipments = new Faker<Shipment>()
    //           .CustomInstantiator(f => new Shipment())
    //           .RuleFor(s => s.ShipmentMethodCode, f => f.Random.Guid().ToString());

    //        var unavailableShipment = testShipments.Generate();
    //        aggregate.Cart.Shipments.Add(unavailableShipment);

    //        // Act
    //        var validator = new CartShipmentValidator(new CartValidationContext().AvailShippingRates);
    //        var result = await validator.ValidateAsync(unavailableShipment, ruleSet: "strict");

    //        // Assert
    //        result.IsValid.Should().BeFalse();
    //        Assert.Single(result.Errors);

    //        Assert.Collection(result.Errors, x =>
    //        {
    //            Assert.Equal(nameof(unavailableShipment.ShipmentMethodCode), x.PropertyName);
    //        });
    //    }

    //    [Fact]
    //    public async Task ValidateShipment_RuleSetStrict_PriceError()
    //    {
    //        // Arrange
    //        var aggregate = await GetValidCartAggregateAsync();
    //        var shipment = Faker.PickRandom(aggregate.Cart.Shipments);
    //        shipment.Price += 1m;
    //        aggregate.Cart.Shipments.Add(shipment);

    //        // Act
    //        var validator = new CartShipmentValidator(new CartValidationContext().AvailShippingRates);
    //        var result = await validator.ValidateAsync(shipment, ruleSet: "strict");

    //        // Assert
    //        result.IsValid.Should().BeFalse();
    //        Assert.Single(result.Errors);

    //        Assert.Collection(result.Errors, x =>
    //        {
    //            Assert.Equal(nameof(shipment.Price), x.PropertyName);
    //        });
    //    }
    //private CartAggregate GetValidCartAggregate()
    //{
    //    var testItems = new Faker<LineItem>()
    //        .CustomInstantiator(f => new LineItem())
    //        .RuleFor(i => i.Id, f => f.Random.Guid().ToString())
    //        .RuleFor(i => i.ListPrice, f => f.Random.Decimal(MIN_PRICE, MAX_PRICE))
    //        .RuleFor(i => i.SalePrice, (f, i) => i.ListPrice);

    //    var testShipments = new Faker<Shipment>()
    //        .CustomInstantiator(f => new Shipment())
    //        .RuleFor(s => s.ShipmentMethodCode, f => f.PickRandom(ShipmentMehodCodes))
    //        .RuleFor(s => s.ShipmentMethodOption, f => "")
    //        .RuleFor(s => s.Price, f => 20);

    //    var cart = GetCart();
    //    cart.Items = testItems.Generate(5).ToList();
    //    cart.Shipments = testShipments.Generate(1).ToList();
    //    cart.CustomerId = Guid.NewGuid().ToString();
    //    cart.CustomerName = Faker.Name.FullName();

    //    var aggregate = new CartAggregate(
    //        _marketingPromoEvaluatorMock.Object,
    //        _shoppingCartTotalsCalculatorMock.Object,
    //        _taxProviderSearchServiceMock.Object,
    //        _mapperMock.Object);

    //    aggregate.GrabCartAsync(cart, new StoreModule.Core.Model.Store(), GetMember(), GetCurrency()).GetAwaiter().GetResult();

    //    return aggregate;
    //}
    //}

    //public class NewCartItemValidatorTests : MoqHelper
    //{
    //    private const int InStockQuantity = 100;

    //    [Fact]
    //    public async Task ValidateAddItem_RuleSetDefault_Valid()
    //    {
    //        // Arrange
    //        var validator = new NewCartItemValidator();
    //        var newCartItem = new NewCartItem(Rand.Guid().ToString(), Rand.Int(1, InStockQuantity));

    //        // Act
    //        var result = await validator.ValidateAsync(newCartItem, ruleSet: "default");

    //        // Assert
    //        result.IsValid.Should().BeTrue();
    //        result.Errors.Should().BeEmpty();
    //    }

    //    [Fact]
    //    public async Task ValidateAddItem_RuleSetDefault_Invalid()
    //    {
    //        // Arrange
    //        var validator = new NewCartItemValidator();
    //        var newCartItem = new NewCartItem(Rand.Guid().ToString(), 0);

    //        // Act
    //        var result = await validator.ValidateAsync(newCartItem, ruleSet: "default");

    //        // Assert
    //        result.IsValid.Should().BeFalse();
    //        result.Errors.Should().NotBeEmpty();
    //        Assert.Collection(result.Errors,
    //            x =>
    //            {
    //                Assert.Equal(nameof(newCartItem.Quantity), x.PropertyName);
    //                Assert.Equal(nameof(GreaterThanValidator), x.ErrorCode);
    //            },
    //            x =>
    //            {
    //                Assert.Equal(nameof(newCartItem.ProductId), x.PropertyName);
    //                Assert.Equal(nameof(NotNullValidator), x.ErrorCode);
    //            }
    //        );
    //    }

    //    [Fact]
    //    public async Task ValidateAddItem_RuleSetStrict_Valid()
    //    {
    //        // Arrange
    //        var productPrice = Rand.Decimal(MIN_PRICE, MAX_PRICE);
    //        var newCartItem = new NewCartItem(Rand.Guid().ToString(), Rand.Int(1, InStockQuantity))
    //        {
    //            Price = productPrice
    //        };
    //        var validator = new NewCartItemValidator();

    //        // Act
    //        var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

    //        // Assert
    //        result.IsValid.Should().BeTrue();
    //        result.Errors.Should().BeEmpty();
    //    }

    //    [Fact]
    //    public async Task ValidateAddItem_RuleSetStrict_PriceError()
    //    {
    //        // Arrange
    //        var productPrice = Rand.Decimal(MIN_PRICE, Rand.Decimal(MIN_PRICE, MAX_PRICE) - 1);
    //        var productId = Rand.Guid().ToString();
    //        var quantity = Rand.Int(1, InStockQuantity);
    //        var newCartItem = BuildNewCartItem(productId, quantity, productPrice);
    //        var validator = new NewCartItemValidator();

    //        // Act
    //        var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

    //        // Assert
    //        result.IsValid.Should().BeFalse();
    //        result.Errors.Should().NotBeEmpty();
    //        Assert.Collection(result.Errors, x =>
    //        {
    //            Assert.Equal(nameof(newCartItem.Price), x.PropertyName);
    //        });
    //    }

    //    [Fact]
    //    public async Task ValidateAddItem_RuleSetStrict_UnavailableQuantity()
    //    {
    //        // Arrange
    //        var validator = new NewCartItemValidator();
    //        var productPrice = Rand.Decimal(MIN_PRICE, MAX_PRICE);
    //        var productId = Rand.Guid().ToString();
    //        var quantity = Rand.Int(InStockQuantity + 1, InStockQuantity * 2);
    //        var newCartItem = BuildNewCartItem(productId, quantity, productPrice);

    //        // Act
    //        var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

    //        // Assert
    //        result.IsValid.Should().BeFalse();
    //        result.Errors.Should().NotBeEmpty();
    //        Assert.Collection(result.Errors, x => { Assert.Equal(nameof(newCartItem.ProductId), x.PropertyName); Assert.Equal(nameof(PredicateValidator), x.ErrorCode); });
    //    }
    //}
}
