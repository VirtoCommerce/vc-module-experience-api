using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Tests.Helpers.Stubs;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Handlers
{
    public class AddOrUpdateCartShipmentCommandHandlerTests : XPurchaseMoqHelper
    {
        [Fact]
        public async Task Handle_RequestWithShipment_AllShipmentFieldsAreMapped()
        {
            // Arragne
            var shipment = _fixture.Create<ExpCartShipment>();

            var cartAggregate = GetValidCartAggregate();
            cartAggregate.Cart.Shipments.Clear();
            shipment.Currency.Value = cartAggregate.Cart.Currency;

            var cartAggregateRepositoryMock = new Mock<ICartAggregateRepository>();
            cartAggregateRepositoryMock
                .Setup(x => x.GetCartByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(cartAggregate);

            var availableShippingMethods = new Mock<ICartAvailMethodsService>();
            availableShippingMethods
                .Setup(x => x.GetAvailableShippingRatesAsync(It.Is<CartAggregate>(y => y == cartAggregate)))
                .ReturnsAsync(new List<ShippingRate>()
                {
                    new ShippingRate()
                    {
                        ShippingMethod = new StubShippingMethod(shipment.ShipmentMethodCode.Value),
                        OptionName = shipment.ShipmentMethodOption.Value,
                        Rate = shipment.Price.Value,
                    }
                });

            var request = new AddOrUpdateCartShipmentCommand()
            {
                Shipment = shipment,
                CartId = cartAggregate.Cart.Id,
            };
            var handler = new AddOrUpdateCartShipmentCommandHandler(cartAggregateRepositoryMock.Object, availableShippingMethods.Object);

            // Act
            var aggregate = await handler.Handle(request, CancellationToken.None);

            // Assert
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.Id == shipment.Id.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.FulfillmentCenterId == shipment.FulfillmentCenterId.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.Length == shipment.Length.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.Height == shipment.Height.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.MeasureUnit == shipment.MeasureUnit.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.ShipmentMethodOption == shipment.ShipmentMethodOption.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.ShipmentMethodCode == shipment.ShipmentMethodCode.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.VolumetricWeight == shipment.VolumetricWeight.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.Weight == shipment.Weight.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.WeightUnit == shipment.WeightUnit.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.Width == shipment.Width.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.Currency == shipment.Currency.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.Price == shipment.Price.Value);
            cartAggregate.Cart.Shipments.Should().ContainSingle(x => x.DeliveryAddress != null);
        }
    }
}
