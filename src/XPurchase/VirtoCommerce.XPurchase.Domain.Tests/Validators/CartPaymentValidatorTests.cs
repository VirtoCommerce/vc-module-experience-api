using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Validators
{
    public class CartPaymentValidatorTests : XPurchaseMoqHelper
    {
        [Fact]
        public async Task ValidatePayment_RuleSetNotStrict_Valid()
        {
            // Arrange
            var item = _fixture.Create<Payment>();

            // Act
            var validator = new CartPaymentValidator();
            var randomRuleSet = _fixture.Create<string>();
            var result = await validator.ValidateAsync(new PaymentValidationContext
            {
                Payment = item,
                AvailPaymentMethods = _context.AvailPaymentMethods
            }, ruleSet: randomRuleSet);

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidatePayment_AvailablePaymentMethodsIsNull_Valid()
        {
            // Arrange
            var item = _fixture.Create<Payment>();

            // Act
            var validator = new CartPaymentValidator();
            var result = await validator.ValidateAsync(new PaymentValidationContext
            {
                 Payment = item
            });

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidatePayment_PaymentGatewayCodeIsNull_Valid()
        {
            // Arrange
            var item = _fixture.Create<Payment>();
            item.PaymentGatewayCode = null;

            // Act
            var validator = new CartPaymentValidator();
            var result = await validator.ValidateAsync(new PaymentValidationContext
            {
                Payment = item,
                AvailPaymentMethods = _context.AvailPaymentMethods
            });

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidatePayment_PaymentGatewayContainingInAvailPaymentMethods_Valid()
        {
            // Arrange
            _context.AvailPaymentMethods = _fixture.CreateMany<PaymentMethod>().ToList();
            var item = _fixture
                .Build<Payment>()
                .With(x => x.PaymentGatewayCode, _context.AvailPaymentMethods.First().Code)
                .Create();

            // Act
            var validator = new CartPaymentValidator();
            var result = await validator.ValidateAsync(new PaymentValidationContext
            {
                Payment = item,
                AvailPaymentMethods = _context.AvailPaymentMethods
            });

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidatePayment_PaymentGatewayNotContainingInAvailPaymentMethods_Valid()
        {
            // Arrange
            _context.AvailPaymentMethods = _fixture.CreateMany<PaymentMethod>().ToList();
            var item = _fixture.Create<Payment>();

            // Act
            var validator = new CartPaymentValidator();
            var result = await validator.ValidateAsync(new PaymentValidationContext
            {
                Payment = item,
                AvailPaymentMethods = _context.AvailPaymentMethods
            });

            // Assert
            var expected = CartErrorDescriber.PaymentMethodUnavailable(item, item.PaymentGatewayCode);
            result.Errors.Should().Contain(x =>
                x.ErrorMessage == expected.ErrorMessage &&
                x.ErrorCode == expected.ErrorCode);
        }
    }
}
