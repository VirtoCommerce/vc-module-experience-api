using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Tests.Helpers;
using VirtoCommerce.ExperienceApiModule.XOrder.Tests.Helpers.Stubs;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.GenericCrud;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests.Handlers
{
    public class AddOrUpdateOrderPaymentCommandHandlerTests : CustomerOrderMockHelper
    {
        [Fact]
        public async Task Handle_RequestWithPayments_AllPaymentFieldsMapped()
        {
            // Arrange
            var payment = _fixture.Create<ExpOrderPayment>();

            var orderAggregate = GetOrderAggregate();
            orderAggregate.Order.InPayments.Clear();

            var orderAggregateRepositoryMock = new Mock<ICustomerOrderAggregateRepository>();
            var customerOrderServiceMock = new Mock<ICustomerOrderService>();
            var paymentMethodsSearchServiceMock = new Mock<IMockPaymentMethodsSearchService>();

            orderAggregateRepositoryMock
                .Setup(x => x.GetOrderByIdAsync(It.Is<string>(x => x == orderAggregate.Order.Id)))
                .ReturnsAsync(orderAggregate);

            paymentMethodsSearchServiceMock
                .Setup(x => x.SearchAsync(It.Is<PaymentMethodsSearchCriteria>(x => x.IsActive.Value && x.StoreId == orderAggregate.Order.StoreId)))
                .ReturnsAsync(new PaymentMethodsSearchResult
                {
                    TotalCount = 1,
                    Results = new List<PaymentMethod>
                    {
                        new StubPaymentMethod(payment.PaymentGatewayCode.Value)
                    }
                });

            var request = new AddOrUpdateOrderPaymentCommand
            {
                Payment = payment,
                OrderId = orderAggregate.Order.Id,
            };
            var handler = new AddOrUpdateOrderPaymentCommandHandler(orderAggregateRepositoryMock.Object, customerOrderServiceMock.Object, paymentMethodsSearchServiceMock.Object);

            // Act
            var aggregate = await handler.Handle(request, CancellationToken.None);

            // Assert
            aggregate.Order.InPayments.Should().ContainSingle(x => x.Id == payment.Id.Value);
            aggregate.Order.InPayments.Should().ContainSingle(x => x.OuterId == payment.OuterId.Value);
            aggregate.Order.InPayments.Should().ContainSingle(x => x.GatewayCode == payment.PaymentGatewayCode.Value);
            aggregate.Order.InPayments.Should().ContainSingle(x => x.Currency == payment.Currency.Value);
            aggregate.Order.InPayments.Should().ContainSingle(x => x.Price == payment.Price.Value);
            aggregate.Order.InPayments.Should().ContainSingle(x => x.Sum == payment.Amount.Value);
            aggregate.Order.InPayments.Should().ContainSingle(x => x.BillingAddress != null);
        }


        // Interface for mocking as IPaymentMethodsSearchService and ISearchService
        public interface IMockPaymentMethodsSearchService : IPaymentMethodsSearchService, ISearchService<PaymentMethodsSearchCriteria, PaymentMethodsSearchResult, PaymentMethod>
        {
        }
    }
}
