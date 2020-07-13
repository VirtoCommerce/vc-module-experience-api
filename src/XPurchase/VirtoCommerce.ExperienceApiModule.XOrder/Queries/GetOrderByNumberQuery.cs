using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class GetOrderByNumberQuery : IQuery<CustomerOrderAggregate>
    {
        public GetOrderByNumberQuery()
        {
        }

        public GetOrderByNumberQuery(string number)
        {
            Number = number;
        }

        public string Number { get; set; }
    }
}
