using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class GetOrderQuery : IQuery<CustomerOrderAggregate>
    {
        public GetOrderQuery()
        {
        }

        public GetOrderQuery(string orderId, string number)
        {
            OrderId = orderId;
            Number = number;
        }

        public string OrderId { get; set; }
        public string Number { get; set; }
    }
}
