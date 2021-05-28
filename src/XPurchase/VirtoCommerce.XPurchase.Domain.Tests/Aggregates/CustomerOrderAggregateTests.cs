using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.XPurchase.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Aggregates
{
    public class CustomerOrderAggregateTests : XPurchaseMoqHelper
    {
        private readonly Mock<IDynamicPropertyUpdaterService> _dynamicPropertyUpdaterServiceMock;

        public CustomerOrderAggregateTests()
        {
            _dynamicPropertyUpdaterServiceMock = new Mock<IDynamicPropertyUpdaterService>();
        }

        [Fact]
        public void CancelPaymentTest_PaymentCancelled()
        {
            //Arrange
            var aggregate = new CustomerOrderAggregate(_dynamicPropertyUpdaterServiceMock.Object);
            aggregate.GrabCustomerOrder(CreateNewOrder(), new Currency(Language.InvariantLanguage, "USD"));
            var existPayment = new PaymentIn()
            {
                Number = "92321873-2E99-44B0-A50C-0A0883C9B137"
            };

            //Act
            var result = aggregate.CancelOrderPayment(existPayment);

            //Assert
            result.Should().BeTrue();
            aggregate.Order.InPayments.Should().HaveCount(1);
            aggregate.Order.InPayments.First().PaymentStatus.Should().Be(PaymentStatus.Cancelled);
            aggregate.Order.InPayments.First().Status.Should().Be(PaymentStatus.Cancelled.ToString());
            aggregate.Order.InPayments.First().IsCancelled.Should().BeTrue();
        }

        [Fact]
        public void CancelPaymentTest_PaymentNotDuplicate_PaymentNotCancelled()
        {
            //Arrange
            var aggregate = new CustomerOrderAggregate(_dynamicPropertyUpdaterServiceMock.Object);
            aggregate.GrabCustomerOrder(CreateNewOrder(), new Currency(Language.InvariantLanguage, "USD"));
            var newPayment = new PaymentIn()
            {
                Number = "newPaymentNumber"
            };

            //Act
            var result = aggregate.CancelOrderPayment(newPayment);

            //Assert
            result.Should().BeFalse();
            aggregate.Order.InPayments.Should().HaveCount(1);
            aggregate.Order.InPayments.First().PaymentStatus.Should().Be(PaymentStatus.New);
            aggregate.Order.InPayments.First().Status.Should().Be(PaymentStatus.New.ToString());
            aggregate.Order.InPayments.First().IsCancelled.Should().BeFalse();
        }

        [Fact]
        public void ConfirmPaymentTest_PaymentConfirmed()
        {
            //Arrange
            var aggregate = new CustomerOrderAggregate(_dynamicPropertyUpdaterServiceMock.Object);
            aggregate.GrabCustomerOrder(CreateNewOrder(), new Currency(Language.InvariantLanguage, "USD"));
            var existPayment = new PaymentIn()
            {
                Number = "92321873-2E99-44B0-A50C-0A0883C9B137"
            };

            //Act
            var result = aggregate.ConfirmOrderPayment(existPayment);

            //Assert
            result.Should().BeTrue();
            aggregate.Order.InPayments.Should().HaveCount(1);
            aggregate.Order.InPayments.First().PaymentStatus.Should().Be(PaymentStatus.Paid);
            aggregate.Order.InPayments.First().Status.Should().Be(PaymentStatus.Paid.ToString());
            aggregate.Order.InPayments.First().IsApproved.Should().BeTrue();
        }

        [Fact]
        public void ConfirmPaymentTest_PaymentNotDuplicate_PaymentNotConfirmed()
        {
            //Arrange
            var aggregate = new CustomerOrderAggregate(_dynamicPropertyUpdaterServiceMock.Object);
            aggregate.GrabCustomerOrder(CreateNewOrder(), new Currency(Language.InvariantLanguage, "USD"));
            var newPayment = new PaymentIn()
            {
                Number = "newPaymentNumber"
            };

            //Act
            var result = aggregate.ConfirmOrderPayment(newPayment);

            //Assert
            result.Should().BeFalse();
            aggregate.Order.InPayments.Should().HaveCount(1);
            aggregate.Order.InPayments.First().PaymentStatus.Should().Be(PaymentStatus.New);
            aggregate.Order.InPayments.First().Status.Should().Be(PaymentStatus.New.ToString());
            aggregate.Order.InPayments.First().IsApproved.Should().BeFalse();
        }

        public CustomerOrder CreateNewOrder()
        {
            return new CustomerOrder()
            {
                Id = "7BF30A9B-4447-448A-85CC-0B943B7F6B21",
                InPayments = new List<PaymentIn>
                {
                    new PaymentIn()
                    {
                        Number = "92321873-2E99-44B0-A50C-0A0883C9B137",
                        PaymentStatus = PaymentStatus.New,
                        Status = PaymentStatus.New.ToString(),
                        IsCancelled = false,
                        IsApproved = false
                    }
                }
            };
        }
    }
}
