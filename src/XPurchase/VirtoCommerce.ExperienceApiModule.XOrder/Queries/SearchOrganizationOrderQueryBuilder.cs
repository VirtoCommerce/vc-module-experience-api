using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrganizationOrderQueryBuilder : BaseSearchOrderQueryBuilder<SearchOrganizationOrderQuery>
    {
        protected override string Name => "organizationOrders";

        public SearchOrganizationOrderQueryBuilder(IMediator mediator, IAuthorizationService authorizationService, ICurrencyService currencyService)
            : base(mediator, authorizationService, currencyService)
        {
        }
    }
}
