using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Validators
{
    public class NewCartItemValidatorTests : XPurchaseMoqHelper
    {
        [Fact]
        public async Task ValidateAddItem_RuleSetDefault_Valid()
        {
            // Arrange
            var validator = new NewCartItemValidator();
            var newCartItem = new NewCartItem(Rand.Guid().ToString(), Rand.Int(1, InStockQuantity))
            {
                CartProduct = _fixture.Create<CartProduct>()
            };

            // Act
            var result = await validator.ValidateAsync(newCartItem, options => options.IncludeRuleSets("default"));

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetDefault_Invalid()
        {
            // Arrange
            var validator = new NewCartItemValidator();
            var newCartItem = new NewCartItem(null, 0);

            // Act
            var result = await validator.ValidateAsync(newCartItem, options => options.IncludeRuleSets("default"));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(3);
            result.Errors.Should().Contain(x => x.PropertyName == "Quantity" && x.ErrorCode.Contains("GreaterThanValidator"));
            result.Errors.Should().Contain(x => x.PropertyName == "ProductId" && x.ErrorCode.Contains("NotNullValidator"));
            result.Errors.Should().Contain(x => x.PropertyName == "CartProduct" && x.ErrorCode.Contains("NotNullValidator"));
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetStrict_Valid()
        {
            // Arrange
            var productPrice = _fixture.Create<decimal>();
            var productId = Rand.Guid().ToString();
            var quantity = Rand.Int(1, InStockQuantity);
            var newCartItemPrice = productPrice * quantity;
            var newCartItem = BuildNewCartItem(productId, quantity, newCartItemPrice, true, true);
            newCartItem.CartProduct.Product.TrackInventory = false;
            var validator = new NewCartItemValidator();

            // Act
            var result = await validator.ValidateAsync(newCartItem, options => options.IncludeRuleSets("strict"));

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetStrict_NoInventory()
        {
            // Arrange
            var productPrice = _fixture.Create<decimal>();
            var productId = Rand.Guid().ToString();
            var quantity = Rand.Int(1, InStockQuantity);
            var newCartItemPrice = productPrice * quantity;
            var validator = new NewCartItemValidator();
            var newCartItem = BuildNewCartItem(productId, quantity, newCartItemPrice, isActive: true, isBuyable: true, trackInventory: true);

            // Act
            var result = await validator.ValidateAsync(newCartItem, options => options.IncludeRuleSets("strict"));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(1);
            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal("PRODUCT_FFC_QTY", x.ErrorCode);
            });
        }

        [Fact]
        public async Task ValidateAddItem_RuleSetStrict_PriceError()
        {
            // Arrange
            var productPrice = Rand.Decimal();
            var productId = Rand.Guid().ToString();
            var quantity = Rand.Int(1, InStockQuantity);
            var newCartItem = BuildNewCartItem(productId, quantity, productPrice, true, true);
            var validator = new NewCartItemValidator();

            // Act
            var result = await validator.ValidateAsync(newCartItem, options => options.IncludeRuleSets("strict"));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Count.Should().Be(1);
            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal("UNABLE_SET_LESS_PRICE", x.ErrorCode);
            });
        }
    }
}
