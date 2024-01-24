using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphQL.Types.Relay;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.XOrder.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public abstract class BaseSearchOrderQueryBuilder<TQuery> : QueryBuilder<TQuery, SearchOrderResponse, CustomerOrderType>
        where TQuery : SearchOrderQuery
    {
        private readonly ICurrencyService _currencyService;

        protected BaseSearchOrderQueryBuilder(IMediator mediator, IAuthorizationService authorizationService, ICurrencyService currencyService)
            : base(mediator, authorizationService)
        {
            _currencyService = currencyService;
        }

        protected override FieldType GetFieldType()
        {
            var builder = GraphTypeExtenstionHelper.CreateConnection<CustomerOrderType, EdgeType<CustomerOrderType>, CustomerOrderConnectionType<CustomerOrderType>, object>()
                .Name(Name)
                .PageSize(20);

            ConfigureArguments(builder.FieldType);

            builder.ResolveAsync(async context =>
            {
                var (query, response) = await Resolve(context);
                return new CustomerOrderConnection<CustomerOrderAggregate>(response.Results, query.Skip, query.Take, response.TotalCount)
                {
                    Facets = response.Facets,
                };
            });

            return builder.FieldType;
        }

        protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, TQuery request)
        {
            context.CopyArgumentsToUserContext();
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            context.SetCurrencies(allCurrencies, request.CultureName);

            await base.BeforeMediatorSend(context, request);
        }

        protected override Task AfterMediatorSend(IResolveFieldContext<object> context, TQuery request, SearchOrderResponse response)
        {
            foreach (var customerOrderAggregate in response.Results)
            {
                context.SetExpandedObjectGraph(customerOrderAggregate);
            }

            return Task.CompletedTask;
        }
    }
}
