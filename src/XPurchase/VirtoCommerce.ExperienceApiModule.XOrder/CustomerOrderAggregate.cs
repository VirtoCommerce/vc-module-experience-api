using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregate 
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
