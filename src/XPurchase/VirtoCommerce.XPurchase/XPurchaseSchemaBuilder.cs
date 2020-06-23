using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Domain.Factories;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase
{
    public class XPurchaseSchemaBuilder : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;
        private readonly IShoppingCartAggregateFactory _cartFactory;

        public XPurchaseSchemaBuilder(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader,
            IShoppingCartAggregateFactory cartFactory)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
            _cartFactory = cartFactory;
        }

        public void Build(ISchema schema)
            => schema.RegisterQueryType<GetCartQueryType>(_cartFactory)
                     .RegisterQueryType<ClearCartQueryType>(_cartFactory);
    }
}
