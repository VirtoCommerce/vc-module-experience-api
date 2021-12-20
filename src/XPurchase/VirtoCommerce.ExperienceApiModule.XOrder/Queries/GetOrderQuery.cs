using GraphQL;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class GetOrderQuery : IQuery<CustomerOrderAggregate>, IExtendableQuery<IResolveFieldContext>
    {
        public GetOrderQuery()
        {
        }

        public GetOrderQuery(string orderId, string number)
        {
            OrderId = orderId;
            Number = number;
        }

        public string CultureName { get; set; }
        public string OrderId { get; set; }
        public string Number { get; set; }

        public virtual void Map(IResolveFieldContext context)
        {
            Number = context.GetArgument<string>("number");
            OrderId = context.GetArgument<string>("id");
            CultureName = context.GetArgument<string>(nameof(Currency.CultureName));
        }
    }
}
