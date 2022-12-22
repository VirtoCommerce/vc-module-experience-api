using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Validators
{
    public class CartValidatorTests : XPurchaseMoqHelper
    {
        private readonly CartValidator _validator = new CartValidator();

        [Fact]
        public async Task ValidateCart_EmptyCart_Valid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();
            aggregate.Cart.Items.Clear();
            aggregate.Cart.Shipments.Clear();
            aggregate.Cart.Payments.Clear();

            // Act
            var result = await _validator.ValidateAsync(new CartValidationContext
            {
                CartAggregate = aggregate
            }, options => options.IncludeRuleSets("default","items","shipments","payments"));

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateCart_RuleSetStrict_Invalid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();
            aggregate.Cart.Name = null;
            aggregate.Cart.CustomerId = null;

            // Act
            var result = await _validator.ValidateAsync(new CartValidationContext
            {
                CartAggregate = aggregate
            }, options => options.IncludeRuleSets("default","items","shipments","payments"));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(8);
        }

        [Fact]
        public async Task ValidateCart_RuleSetDefault_Valid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();

            // Act
            var result = await _validator.ValidateAsync(new CartValidationContext
            {
                CartAggregate = aggregate
            }, options => options.IncludeRuleSets("default"));

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateCart_ApplyRuleOverride()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();

            var validator2 = new CartValidator2();

            // Act
            var result = await validator2.ValidateAsync(new CartValidationContext
            {
                CartAggregate = aggregate
            }, options => options.IncludeRuleSets("items"));

            // Assert
            result.Errors.Should().Contain(x => x.ErrorMessage == "FakeFailure");
        }
    }
}
