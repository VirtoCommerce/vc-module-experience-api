using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Tests.Helpers.Stubs;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Validators
{
    public class CartShipmentValidatorTests : XPurchaseMoqHelper
    {
        private readonly ShippingRate _shippingRate;

        public CartShipmentValidatorTests()
        {
            _shippingRate = new ShippingRate
            {
                OptionName = ":)",
                ShippingMethod = new StubShippingMethod("shippingMethodCode"),
                Rate = 777,
            };

            _context.AvailShippingRates = new List<ShippingRate>()
            {
                _shippingRate
            };
        }
               

        [Fact]
        public async Task ValidateShipment_RuleSetDefault_ShipmentMethodCodeIsNull_Valid()
        {
            // Arrange
            var shipment = new CartModule.Core.Model.Shipment
            {
                ShipmentMethodCode = null
            };

            // Act
            var validator = new CartShipmentValidator();
            var result = await validator.ValidateAsync(new ShipmentValidationContext
            {
                Shipment = shipment,
                AvailShippingRates = _context.AvailShippingRates
            });

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateShipment_RuleSetDefault_ShipmentMethodCodeIsEmpty_Valid()
        {
            // Arrange
            var shipment = new CartModule.Core.Model.Shipment
            {
                ShipmentMethodCode = string.Empty
            };

            // Act
            var validator = new CartShipmentValidator();
            var result = await validator.ValidateAsync(new ShipmentValidationContext
            {
                Shipment = shipment,
                AvailShippingRates = _context.AvailShippingRates
            });

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateShipment_RuleSetDefault_UnavailableMethodError()
        {
            // Arrange
            var shipment = new CartModule.Core.Model.Shipment
            {
                ShipmentMethodCode = "UnavailableShipmentMethod"
            };

            // Act
            var validator = new CartShipmentValidator();
            var result = await validator.ValidateAsync(new ShipmentValidationContext
            {
                Shipment = shipment,
                AvailShippingRates = _context.AvailShippingRates
            });

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(x => x.ErrorCode == "SHIPMENT_METHOD_UNAVAILABLE");
        }

        [Fact]
        public async Task ValidateShipment_RuleSetDefault_PriceError()
        {
            // Arrange
            var shipment = new CartModule.Core.Model.Shipment
            {
                ShipmentMethodCode = "shippingMethodCode",
                ShipmentMethodOption = ":)",
            };

            shipment.Price = _shippingRate.Rate + 1;

            // Act
            var validator = new CartShipmentValidator();
            var result = await validator.ValidateAsync(new ShipmentValidationContext
            {
                Shipment = shipment,
                AvailShippingRates = _context.AvailShippingRates
            });

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(x => x.ErrorCode == "SHIPMENT_METHOD_PRICE_CHANGED");
        }
    }
}
