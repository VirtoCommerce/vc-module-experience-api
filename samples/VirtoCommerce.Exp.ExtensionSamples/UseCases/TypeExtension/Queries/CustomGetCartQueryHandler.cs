using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Queries;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class CustomGetCartQueryHandler : GetCartQueryHandler
    {
        public CustomGetCartQueryHandler(ICartAggregateRepository cartAggregateRepository,
            ICartResponseGroupParser cartResponseGroupParser)
            : base(cartAggregateRepository, cartResponseGroupParser)
        {
        }

        public override async Task<CartAggregate> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var result = await base.Handle(request, cancellationToken);

            result.Cart.ChannelId = "my-cool-channel";

            return result;
        }

        protected override ShoppingCartSearchCriteria GetCartSearchCriteria(GetCartQuery request)
        {
            var requestExtended = (GetCartQueryExtended)request;
            var criteriaExtended = (ShoppingCartSearchCriteriaExtended)base.GetCartSearchCriteria(request);

            criteriaExtended.ContractId = requestExtended.ContractId;

            return criteriaExtended;
        }
    }
}
