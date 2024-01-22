using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
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
    public class SearchOrderQuery : Query<SearchOrderResponse>
    {
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Facet { get; set; }
        public string CultureName { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<StringGraphType>(nameof(Facet), "This parameter applies a facet to the query results");
            yield return Argument<StringGraphType>(nameof(Filter), "This parameter applies a filter to the query results");
            yield return Argument<StringGraphType>(nameof(Sort), "The sort expression");
            yield return Argument<StringGraphType>(nameof(CultureName), "Culture name (\"en-US\")");
        }

        public override void Map(IResolveFieldContext context)
        {
            if (context is IResolveConnectionContext connectionContext)
            {
                Skip = int.TryParse(connectionContext.After, out var skip) ? skip : 0;
                Take = connectionContext.First ?? connectionContext.PageSize ?? 20;
            }

            CultureName = context.GetArgument<string>(nameof(CultureName));
            Filter = context.GetArgument<string>(nameof(Filter));
            Facet = context.GetArgument<string>(nameof(Facet));
            Sort = context.GetArgument<string>(nameof(Sort));
        }
    }

    public class SearchOrderQueryBuilder : SearchOrderQueryBuilder<SearchCustomerOrderQuery>
    {
        protected override string Name => "orders";

        public SearchOrderQueryBuilder(IMediator mediator, IAuthorizationService authorizationService, ICurrencyService currencyService)
            : base(mediator, authorizationService, currencyService)
        {
        }
    }

    public class SearchOrganizationOrderQueryBuilder : SearchOrderQueryBuilder<SearchOrganizationOrderQuery>
    {
        protected override string Name => "organizationOrders";

        public SearchOrganizationOrderQueryBuilder(IMediator mediator, IAuthorizationService authorizationService, ICurrencyService currencyService)
            : base(mediator, authorizationService, currencyService)
        {
        }
    }

    public abstract class SearchOrderQueryBuilder<TQuery> : QueryBuilder<TQuery, SearchOrderResponse, CustomerOrderType>
        where TQuery : SearchOrderQuery
    {
        private readonly ICurrencyService _currencyService;

        protected SearchOrderQueryBuilder(IMediator mediator, IAuthorizationService authorizationService, ICurrencyService currencyService)
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
