using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
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
            var result = await validator.ValidateAsync(newCartItem, ruleSet: "default");

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
            var result = await validator.ValidateAsync(newCartItem, ruleSet: "default");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(3);
            result.Errors.Should().Contain(x => x.PropertyName == "Quantity" && x.ErrorCode == nameof(GreaterThanValidator));
            result.Errors.Should().Contain(x => x.PropertyName == "ProductId" && x.ErrorCode == nameof(NotNullValidator));
            result.Errors.Should().Contain(x => x.PropertyName == "CartProduct" && x.ErrorCode == nameof(NotNullValidator));
        }

        // TODO: Implement this!
        //[Fact]
        //public async Task ValidateAddItem_RuleSetStrict_Valid()
        //{
        //    // Arrange
        //    var productPrice = _fixture.Create<decimal>();
        //    var newCartItem = new NewCartItem(Rand.Guid().ToString(), Rand.Int(1, InStockQuantity))
        //    {
        //        Price = productPrice
        //    };
        //    var validator = new NewCartItemValidator();

        //    // Act
        //    var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

        //    // Assert
        //    result.IsValid.Should().BeTrue();
        //    result.Errors.Should().BeEmpty();
        //}

        //[Fact]
        //public async Task ValidateAddItem_RuleSetStrict_PriceError()
        //{
        //    // Arrange
        //    var productPrice = Rand.Decimal(1, _fixture.Create<decimal>() - 1);
        //    var productId = Rand.Guid().ToString();
        //    var quantity = Rand.Int(1, InStockQuantity);
        //    var newCartItem = BuildNewCartItem(productId, quantity, productPrice);
        //    var validator = new NewCartItemValidator();

        //    // Act
        //    var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

        //    // Assert
        //    result.IsValid.Should().BeFalse();
        //    result.Errors.Should().NotBeEmpty();
        //    Assert.Collection(result.Errors, x =>
        //    {
        //        Assert.Equal(nameof(newCartItem.Price), x.PropertyName);
        //    });
        //}

        //[Fact]
        //public async Task ValidateAddItem_RuleSetStrict_UnavailableQuantity()
        //{
        //    // Arrange
        //    var validator = new NewCartItemValidator();
        //    var productPrice = _fixture.Create<decimal>();
        //    var productId = Rand.Guid().ToString();
        //    var quantity = Rand.Int(InStockQuantity + 1, InStockQuantity * 2);
        //    var newCartItem = BuildNewCartItem(productId, quantity, productPrice);

        //    // Act
        //    var result = await validator.ValidateAsync(newCartItem, ruleSet: "strict");

        //    // Assert
        //    result.IsValid.Should().BeFalse();
        //    result.Errors.Should().NotBeEmpty();
        //    Assert.Collection(result.Errors, x => { Assert.Equal(nameof(newCartItem.ProductId), x.PropertyName); Assert.Equal(nameof(PredicateValidator), x.ErrorCode); });
        //}
    }
}
