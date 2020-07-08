using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Validators
{
    public class ChangeCartItemPriceValidatorTests : MoqHelper
    {
        [Fact]
        public async Task ValidateChangePriceItem_RuleSetDefault_Valid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();
            var newItemPrice = new PriceAdjustment(_fixture.Create<string>(), _fixture.Create<decimal>());
            var validator = new ChangeCartItemPriceValidator(aggregate);

            // Act
            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "default");

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetDefault_Invalid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();

            var newItemPrice = new PriceAdjustment(null, -1);

            // Act
            var validator = new ChangeCartItemPriceValidator(aggregate);
            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "default");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(3);
            result.Errors.Should().Contain(x => x.PropertyName == "NewPrice" && x.ErrorCode == nameof(GreaterThanOrEqualValidator));
            result.Errors.Should().Contain(x => x.PropertyName == "LineItemId" && x.ErrorCode == nameof(NotEmptyValidator));
            result.Errors.Should().Contain(x => x.PropertyName == "LineItemId" && x.ErrorCode == nameof(NotNullValidator));
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetStrict_Valid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();
            var item = _fixture.Create<LineItem>();
            aggregate.Cart.Items = new List<LineItem> { item };

            var newItemPrice = new PriceAdjustment(item.Id, item.ListPrice + _fixture.Create<decimal>());
            var validator = new ChangeCartItemPriceValidator(aggregate);

            // Act
            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "strict");

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetStrict_Invalid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();
            var item = _fixture.Create<LineItem>();
            aggregate.Cart.Items = new List<LineItem> { item };

            var newItemPrice = new PriceAdjustment(item.Id, item.ListPrice - _fixture.Create<decimal>());
            var validator = new ChangeCartItemPriceValidator(aggregate);

            // Act
            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "strict");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(x => x.ErrorCode == "UNABLE_SET_LESS_PRICE");
        }
    }
}
