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
