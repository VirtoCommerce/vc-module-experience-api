using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregate
    {
        public CustomerOrderAggregate(CustomerOrder order)
        {
            Order = order;
        }

        public CustomerOrder Order { get; protected set; }
    }
}
