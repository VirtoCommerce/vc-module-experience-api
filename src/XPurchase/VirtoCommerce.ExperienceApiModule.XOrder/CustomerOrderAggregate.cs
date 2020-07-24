using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregate : Entity, IAggregateRoot
    {

        public CustomerOrderAggregate(CustomerOrder order, Currency currency)
        {
            Order = order;
            Currency = currency;
        }

        public CustomerOrder Order { get; protected set; }
        public Currency Currency { get; protected set; }                
    }
}
