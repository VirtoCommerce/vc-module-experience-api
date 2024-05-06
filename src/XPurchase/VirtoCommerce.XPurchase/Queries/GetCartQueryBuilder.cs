using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XPurchase.Authorization;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Queries
{
    public class GetCartQueryBuilder : QueryBuilder<GetCartQuery, CartAggregate, CartType>
    {
        protected override string Name => "cart";

        private readonly IMediator _mediator;
        private readonly ICurrencyService _currencyService;
        private readonly IUserManagerCore _userManagerCore;

        public GetCartQueryBuilder(
            IMediator mediator,
            IAuthorizationService authorizationService,
            ICurrencyService currencyService,
            IUserManagerCore userManagerCore)
            : base(mediator, authorizationService)
        {
            _mediator = mediator;
            _currencyService = currencyService;
            _userManagerCore = userManagerCore;
        }

        protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, GetCartQuery request)
        {
            context.CopyArgumentsToUserContext();

            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            //Store all currencies in the user context for future resolve in the schema types
            //this is required to resolve Currency in DiscountType
            context.SetCurrencies(allCurrencies, request.CultureName);
        }

        protected override async Task<CartAggregate> GetResponseAsync(IResolveFieldContext<object> context, GetCartQuery request)
        {
            var response = await base.GetResponseAsync(context, request);

            var cartAuthorizationRequirement = new CanAccessCartAuthorizationRequirement();

            if (response == null)
            {
                await Authorize(context, request.UserId, cartAuthorizationRequirement);

                var createCartCommand = new CreateCartCommand(request.StoreId, request.CartType, request.CartName, request.UserId, request.CurrencyCode, request.CultureName);
                response = await _mediator.Send(createCartCommand);
            }
            else
            {
                await Authorize(context, response.Cart, cartAuthorizationRequirement);
            }

            return response;
        }

        protected override Task AfterMediatorSend(IResolveFieldContext<object> context, GetCartQuery request, CartAggregate response)
        {
            context.SetExpandedObjectGraph(response);
            return Task.CompletedTask;
        }

        protected override async Task Authorize(IResolveFieldContext context, object resource, IAuthorizationRequirement requirement)
        {
            await _userManagerCore.CheckUserState(context.GetCurrentUserId(), allowAnonymous: true);

            await base.Authorize(context, resource, requirement);
        }
    }
}
