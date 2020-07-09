using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Validators
{
    public class CartValidatorTests : MoqHelper
    {
        private readonly CartValidator _validator = new CartValidator(new CartValidationContext());

        [Fact]
        public async Task ValidateCart_RuleSetDefault_Valid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();

            // Act
            var result = await _validator.ValidateAsync(aggregate, ruleSet: "default");

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateCart_RuleSetDefault_Invalid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();
            aggregate.Cart.Name = null;
            aggregate.Cart.CustomerId = null;

            // Act
            var result = await _validator.ValidateAsync(aggregate, ruleSet: "default");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(4);
            result.Errors.Should().Contain(x => x.PropertyName == "Cart.Name" && x.ErrorCode == nameof(NotNullValidator));
            result.Errors.Should().Contain(x => x.PropertyName == "Cart.Name" && x.ErrorCode == nameof(NotEmptyValidator));
            result.Errors.Should().Contain(x => x.PropertyName == "Cart.CustomerId" && x.ErrorCode == nameof(NotNullValidator));
            result.Errors.Should().Contain(x => x.PropertyName == "Cart.CustomerId" && x.ErrorCode == nameof(NotEmptyValidator));
        }
    }
}
