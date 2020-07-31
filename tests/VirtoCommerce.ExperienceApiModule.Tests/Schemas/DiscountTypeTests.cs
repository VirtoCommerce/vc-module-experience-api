using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Tests.Helpers;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using Xunit;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using System;

namespace VirtoCommerce.ExperienceApiModule.Tests.Schemas
{
    public class DiscountTypeTests : MoqHelper
    {
        private readonly DiscountType _discountType;

        public DiscountTypeTests()
        {
            _discountType = new DiscountType();
        }

        [Fact]
        public void DiscountType_ShouldHavePropperFieldAmount()
        {
            // Assert
            _discountType.Fields.Should().HaveCount(5);
        }

        [Fact]
        public void DiscountType_Coupon_ShouldResolve()
        {
            // Arrange
            var discount = GetDiscount();
            var resolveContext = new ResolveFieldContext()
            {
                Source = discount
            };

            // Act
            var result = _discountType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("Coupon")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<string>();
            ((string)result).Should().Be(discount.Coupon);
        }

        [Fact]
        public void DiscountType_Description_ShouldResolve()
        {
            // Arrange
            var discount = GetDiscount();
            var resolveContext = new ResolveFieldContext()
            {
                Source = discount
            };

            // Act
            var result = _discountType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("Description")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<string>();
            ((string)result).Should().Be(discount.Description);
        }

        [Fact]
        public void DiscountType_PromotionId_ShouldResolve()
        {
            // Arrange
            var discount = GetDiscount();
            var resolveContext = new ResolveFieldContext()
            {
                Source = discount
            };

            // Act
            var result = _discountType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("PromotionId")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<string>();
            ((string)result).Should().Be(discount.PromotionId);
        }

        [Fact]
        public void DiscountType_Amount_ShouldResolve()
        {
            // Arrange
            var discount = GetDiscount();
            var resolveContext = new ResolveFieldContext()
            {
                Source = discount,
                Arguments = new Dictionary<string, object>
                {
                    { Constants.CurrencyCode, CURRENCY_CODE },
                    { Constants.CultureName, CULTURE_NAME }
                },
                UserContext = new Dictionary<string, object>()
            };

            // Act
            var result = _discountType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("Amount")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<Money>();
            ((Money)result).Should().Be(discount.DiscountAmount.ToMoney(GetCurrency()));
        }

        [Fact]
        public void DiscountType_Amount_ArgumentsNotPassed_ShouldBeNull()
        {
            // Arrange
            var discount = GetDiscount();
            var resolveContext = new ResolveFieldContext()
            {
                Source = discount,
                Arguments = new Dictionary<string, object>(),
                UserContext = new Dictionary<string, object>()
            };

            // Act
            var result = _discountType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("Amount")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DiscountType_AmountWithTax_ShouldResolve()
        {
            // Arrange
            var discount = GetDiscount();
            var resolveContext = new ResolveFieldContext()
            {
                Source = discount,
                Arguments = new Dictionary<string, object>
                {
                    { Constants.CurrencyCode, CURRENCY_CODE },
                    { Constants.CultureName, CULTURE_NAME }
                },
                UserContext = new Dictionary<string, object>()
            };

            // Act
            var result = _discountType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("AmountWithTax")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<Money>();
            ((Money)result).Should().Be(discount.DiscountAmountWithTax.ToMoney(GetCurrency()));
        }

        [Fact]
        public void DiscountType_AmountWithTax_ArgumentsNotPassed_ShouldBeNull()
        {
            // Arrange
            var discount = GetDiscount();
            var resolveContext = new ResolveFieldContext()
            {
                Source = discount,
                Arguments = new Dictionary<string, object>(),
                UserContext = new Dictionary<string, object>()
            };

            // Act
            var result = _discountType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("AmountWithTax")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeNull();
        }
    }
}
