using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public abstract class CatalogQueryBuilder<TQuery, TResult, TResultGraphType>
    : QueryBuilder<TQuery, TResult, TResultGraphType>
    where TQuery : CatalogQueryBase<TResult>
    where TResultGraphType : IGraphType
{
    private readonly IStoreService _storeService;
    private readonly ICurrencyService _currencyService;

    protected CatalogQueryBuilder(
        IMediator mediator,
        IAuthorizationService authorizationService,
        IStoreService storeService,
        ICurrencyService currencyService)
        : base(mediator, authorizationService)
    {
        _storeService = storeService;
        _currencyService = currencyService;
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, TQuery request)
    {
        await base.BeforeMediatorSend(context, request);

        request.IncludeFields = context.SubFields?.Values.GetAllNodesPaths(context).ToArray() ?? Array.Empty<string>();

        if (!string.IsNullOrEmpty(request.StoreId))
        {
            var store = await _storeService.GetByIdAsync(request.StoreId);
            request.Store = store;
            context.UserContext["store"] = store;
            context.UserContext["catalog"] = store.Catalog;
        }

        // PT-1606: Need to ensure there is no alternative way to access original request arguments in sub selection
        context.CopyArgumentsToUserContext();

        var currencies = await _currencyService.GetAllCurrenciesAsync();
        context.SetCurrencies(currencies, request.CultureName);
    }
}
