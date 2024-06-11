using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Authorization;
using VirtoCommerce.XPurchase.Core.Models;
using VirtoCommerce.XPurchase.Core.Queries;
using VirtoCommerce.XPurchase.Core.Schemas;

namespace VirtoCommerce.XPurchase.Data.Queries
{
    public class SearchCartQueryBuilder : SearchQueryBuilder<SearchCartQuery, SearchCartResponse, CartAggregate, CartType>
    {
        protected override string Name => "carts";

        private readonly ICurrencyService _currencyService;
        private readonly IUserManagerCore _userManagerCore;

        public SearchCartQueryBuilder(
            IMediator mediator,
            IAuthorizationService authorizationService,
            ICurrencyService currencyService,
            IUserManagerCore userManagerCore)
            : base(mediator, authorizationService)
        {
            _currencyService = currencyService;
            _userManagerCore = userManagerCore;
        }

        protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, SearchCartQuery request)
        {
            context.CopyArgumentsToUserContext();

            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            //Store all currencies in the user context for future resolve in the schema types
            //this is required to resolve Currency in DiscountType
            context.SetCurrencies(allCurrencies, request.CultureName);

            await Authorize(context, request, new CanAccessCartAuthorizationRequirement());
        }

        protected override Task AfterMediatorSend(IResolveFieldContext<object> context, SearchCartQuery request, SearchCartResponse response)
        {
            foreach (var cartAggregate in response.Results)
            {
                context.SetExpandedObjectGraph(cartAggregate);
            }

            return Task.CompletedTask;
        }

        protected override async Task Authorize(IResolveFieldContext context, object resource, IAuthorizationRequirement requirement)
        {
            await _userManagerCore.CheckUserState(context.GetCurrentUserId(), allowAnonymous: true);

            await base.Authorize(context, resource, requirement);
        }
    }

}
