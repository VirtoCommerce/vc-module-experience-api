using System.Threading.Tasks;
using FluentAssertions;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Services
{
    public class CartAvailMethodsServiceTests : XPurchaseMoqHelper
    {
        private readonly CartAvailMethodsService service;

        public CartAvailMethodsServiceTests()
        {
            service = new CartAvailMethodsService(
                _paymentMethodsSearchServiceMock.Object,
                _shippingMethodsSearchServiceMock.Object,
                _taxProviderSearchServiceMock.Object,
                _cartProductServiceMock.Object,
                _mapperMock.Object);
        }

        #region GetAvailableShippingRatesAsync

        [Fact]
        public async Task GetAvailableShippingRatesAsync_AggregateIsNull_ShouldReturnEmptyResult()
        {
            // Arrange
            CartAggregate cartAggregate = null;

            // Act
            var result = await service.GetAvailableShippingRatesAsync(cartAggregate);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion GetAvailableShippingRatesAsync

        #region GetAvailablePaymentMethodsAsync

        [Fact]
        public async Task GetAvailablePaymentMethodsAsync_AggregateIsNull_ShouldReturnEmptyResult()
        {
            // Arrange
            CartAggregate cartAggregate = null;

            // Act
            var result = await service.GetAvailablePaymentMethodsAsync(cartAggregate);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion GetAvailablePaymentMethodsAsync
    }
}
