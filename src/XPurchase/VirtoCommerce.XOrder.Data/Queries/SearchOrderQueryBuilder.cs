using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XOrder.Core.Queries;
using VirtoCommerce.XOrder.Core.Queries.BaseQueries;

namespace VirtoCommerce.XOrder.Data.Queries
{
    public class SearchOrderQueryBuilder : BaseSearchOrderQueryBuilder<SearchCustomerOrderQuery>
    {
        protected override string Name => "orders";

        public SearchOrderQueryBuilder(IMediator mediator, IAuthorizationService authorizationService, ICurrencyService currencyService, IUserManagerCore userManagerCore)
            : base(mediator, authorizationService, currencyService, userManagerCore)
        {
        }
    }
}
