using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class GetOrderByIdQuery : IQuery<CustomerOrderAggregate>
    {
        public GetOrderByIdQuery()
        {
        }

        public GetOrderByIdQuery(string orderId)
        {
            OrderId = orderId;
        }

        public string OrderId { get; set; }
    }
}
